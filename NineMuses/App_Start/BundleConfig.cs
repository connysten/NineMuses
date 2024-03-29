﻿using System.Web;
using System.Web.Optimization;

namespace NineMuses
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/JS/Ajax").Include(
                      "~/Scripts/Ajax.js",
                      "~/Scripts/polyfill.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/icons.css",
                      "~/Content/Layout.css",
                      "~/Content/index.css",
                      "~/Content/assets.css",
                      "~/Content/view.css",
                      "~/Content/sign-in-sign.up.css",
                      "~/Content/search.css",
                      "~/Content/upload.css",
                      "~/Content/profile.css"));
        }
    }
}
