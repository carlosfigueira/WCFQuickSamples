#region Copyright Notice
// ----------------------------------------------------------------------------
// Copyright (C) 2006 Microsoft Corporation, All rights reserved.
// ----------------------------------------------------------------------------

// Author: Vipul Modi (vipul.modi@microsoft.com)
#endregion

namespace WcfSamples.DynamicProxy
{
    using System;
    using System.Text;

    public delegate string ProxyCodeModifier(string proxyCode);

    public class DynamicProxyFactoryOptions
    {
        public enum LanguageOptions { CS, VB };
        public enum FormatModeOptions { Auto, XmlSerializer, DataContractSerializer }

        private LanguageOptions lang;
        private FormatModeOptions mode;
        private ProxyCodeModifier codeModifier;

        public DynamicProxyFactoryOptions()
        {
            this.lang = LanguageOptions.CS;
            this.mode = FormatModeOptions.Auto;
            this.codeModifier = null;
        }

        public LanguageOptions Language
        {
            get
            {
                return this.lang;
            }

            set
            {
                this.lang = value;
            }
        }

        public FormatModeOptions FormatMode
        {
            get
            {
                return this.mode;
            }

            set
            {
                this.mode = value;
            }
        }

        // The code modifier allows the user of the dynamic proxy factory to modify 
        // the generated proxy code before it is compiled and used. This is useful in 
        // situations where the generated proxy has to be modified manually for interop 
        // reason.
        public ProxyCodeModifier CodeModifier
        {
            get
            {
                return this.codeModifier;
            }

            set
            {
                this.codeModifier = value;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DynamicProxyFactoryOptions[");
            sb.Append("Language=" + Language);
            sb.Append(",FormatMode=" + FormatMode);
            sb.Append(",CodeModifier=" + CodeModifier);
            sb.Append("]");

            return sb.ToString();
        }
    }
}
