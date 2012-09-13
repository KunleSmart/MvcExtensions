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

using System;
using System.Web;
using System.Web.Mvc;

public static class MyStyle
{
    public static IHtmlString Render(string style)
    {
        TagBuilder builder = new TagBuilder("style");
        builder.Attributes["type"] = "text/css";
        builder.InnerHtml = style;
        return new MvcHtmlString(builder.ToString(TagRenderMode.Normal));
    }
}