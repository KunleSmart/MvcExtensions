//------------------------------------------------------------------------------------------
//THIS CODE CONSTITUTES INTELLECTUAL PROPERTY AND IS FULLY OWNED BY BYAXIOM SOLUTIONS LIMITED
//NO PORTION OF IT MAY BE TRANSMITTED OR MADE AVAILABLE IN ANY FORM OR MEDIA WITHOUT FULL
//AUTHORIZATION (IN WRITING) FROM BYAXIOM SOLUTIONS LIMITED.
//Copyright (c) 2012 BYAXIOM SOLUTIONS LIMITED.  All Rights Reserved
//Module      : HtmlExtension.cs
//DateTime    : 27-Jan-2012 07:01:AM
//Author      : Ishmail A. Rahman
//Position    : Lead Developer
//Purpose     : 
//Procedures  : 
//Assumptions : 
//Comment     : 
//------------------------------------------------------------------------------------------

using Byaxiom.MvcExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

public static class HtmlExtension
{
    private enum UReturn
    {
        NoChange = 1,
        ConcurrencyViolation = 2,
        ItemCreated = 3,
        ItemUpdated = 4,
        ItemDeleted = 5,
        PasswordReset = 6,
        PasswordChanged = 7,
        ValidationError = 8,
        PasswordValidationError = 9,
        UnknownError = 10
    }

    public static IHtmlString TextBoxTag(this HtmlHelper html, string name)
    {
        TagBuilder builder = new TagBuilder("input");
        builder.Attributes["type"] = "text";
        builder.Attributes["id"] = name;
        builder.Attributes["name"] = name;
        builder.Attributes["value"] = html.ViewData[name].ToString();

        return new MvcHtmlString(builder.ToString(TagRenderMode.Normal));
    }

    public static IHtmlString Message(this HtmlHelper html, int? message, string url = "")
    {
        message = message ?? -1;
        url = url ?? "";
        UReturn r = (UReturn)message;

        TagBuilder builder = new TagBuilder("span");
        builder.Attributes["id"] = "update-message";
        builder.Attributes["data-message"] = message.ToString();

        switch (r)
        {
            case UReturn.NoChange:
                builder.Attributes["style"] = "color:blue";
                builder.SetInnerText("Record was not modified for update.");
                break;

            case UReturn.ConcurrencyViolation:
                builder.Attributes["style"] = "color:red";
                builder.InnerHtml = "This record has been modified or deleted in another instance. You may <a href=\"" + url + "\"> reload</a> this page to re-attempt.";
                break;

            case UReturn.ItemCreated:
                builder.Attributes["style"] = "color:green";
                builder.SetInnerText("Record was successfuly created.");
                break;

            case UReturn.ItemUpdated:
                builder.Attributes["style"] = "color:green";
                builder.SetInnerText("Record was successfuly updated.");
                break;

            case UReturn.ItemDeleted:
                builder.Attributes["style"] = "color:green";
                builder.SetInnerText("Record was successfuly deleted.");
                break;

            case UReturn.PasswordReset:
                builder.Attributes["style"] = "color:green";
                builder.SetInnerText("Password was successfuly reset.");
                break;

            case UReturn.PasswordChanged:
                builder.Attributes["style"] = "color:green";
                builder.SetInnerText("Password was successfuly changed.");
                break;

            case UReturn.ValidationError:
                builder.Attributes["style"] = "color:red";
                builder.SetInnerText("There was a problem validating transaction.");
                break;

            case UReturn.PasswordValidationError:
                builder.Attributes["style"] = "color:red";
                builder.SetInnerText("There was a problem validating your password.");
                break;

            case UReturn.UnknownError:
                builder.Attributes["style"] = "color:red";
                builder.SetInnerText("There was a problem updating record. Please try again.");
                break;

            default:
                break;

        }

        return new MvcHtmlString(builder.ToString(TagRenderMode.Normal));

    }

    public static string RemoveSpace(this HtmlHelper html, string name)
    {
        string[] _name = name.Split(' ');
        name = string.Empty;
        foreach (string s in _name)
            name += s;
        return name;
    }

    /// <summary>
    /// Developed by Adam Tuliper
    /// www.secure-coding.com
    /// adam@secure-coding.com
    /// Uses MvcHtmlString for backwards compatibility. 
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public static MvcHtmlString AntiModelInjection(this HtmlHelper html, string modelPropertyName, object value)
    {
        return GenerateHiddenFormField(modelPropertyName, value);
    }

    /// <summary>
    /// Generates a hidden form field with a hashed value of the value in the model.
    /// This is used in conjunction with [ValidateAntiModelInjection] upon a form post to help ensure
    /// the form fields weren't tampered with.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="htmlHelper"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static MvcHtmlString AntiModelInjectionFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
    {
        //Get the value from the model.
        object modelValue = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model;

        //Get the name of the field from the model.
        string fieldName = ExpressionHelper.GetExpressionText(expression);

        return GenerateHiddenFormField(fieldName, modelValue);
    }

    /// <summary>
    /// Generates a hidden input type string for the given value after hashing the value.
    /// The field for ex. CustomerId would be named _CustomerIdToken
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="modelValue"></param>
    /// <returns></returns>
    private static MvcHtmlString GenerateHiddenFormField(string fieldName, object modelValue)
    {
        TagBuilder builder = new TagBuilder("input");
        builder.Attributes["type"] = "hidden";
        //If we have a field named CustomerId, then the token will be _CustomerIdToken
        builder.Attributes["name"] = string.Format("_{0}Token", fieldName);

        string value = GetValueFromModelValue(modelValue);

        //Now hash the value
        //value = FormsAuthentication.HashPasswordForStoringInConfigFile(value, "SHA1");
        value = SimpleHash.ComputeHash(value, "SHA256", null);

        builder.Attributes["value"] = value.ToString();
        return new MvcHtmlString(builder.ToString(TagRenderMode.SelfClosing));

    }

    /// <summary>
    /// Gets the value from the model as a string. Binary types are converted to base 64 strings.
    /// </summary>
    /// <param name="formValue"></param>
    /// <returns></returns>
    private static string GetValueFromModelValue(object formValue)
    {
        //Test to determine if its binary data. If it is, we need to convert it to a base64 string.
        Binary binaryValue = formValue as Binary;
        if (binaryValue != null)
        {
            formValue = binaryValue.ToArray();
        }

        //If the above conversion to an array worked, then the following will cast as a byte array and convert.
        byte[] byteArrayValue = formValue as byte[];
        if (byteArrayValue != null)
        {
            formValue = Convert.ToBase64String(byteArrayValue);
        }
        return formValue.ToString();
    }

    private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
    {
        Type realModelType = modelMetadata.ModelType;

        Type underlyingType = Nullable.GetUnderlyingType(realModelType);
        if (underlyingType != null)
        {
            realModelType = underlyingType;
        }
        return realModelType;
    }

    private static readonly SelectListItem[] SingleEmptyItem = new[] { new SelectListItem { Text = "", Value = "" } };

    public static string GetEnumDescription<TEnum>(TEnum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

        if ((attributes != null) && (attributes.Length > 0))
            return attributes[0].Description;
        else
            return value.ToString();
    }

    public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
    {
        return EnumDropDownListFor(htmlHelper, expression, null);
    }

    public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes)
    {
        ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
        Type enumType = GetNonNullableModelType(metadata);
        IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

        IEnumerable<SelectListItem> items = from value in values
                                            select new SelectListItem
                                            {
                                                Text = GetEnumDescription(value),
                                                Value = value.ToString(),
                                                Selected = value.Equals(metadata.Model)
                                            };

        // If the enum is nullable, add an 'empty' item to the collection
        if (metadata.IsNullableValueType)
            items = SingleEmptyItem.Concat(items);

        return htmlHelper.DropDownListFor(expression, items, htmlAttributes);
    }

}