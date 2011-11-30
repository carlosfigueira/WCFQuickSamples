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
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ServiceModel.Description;
    
    public class DynamicProxyException : ApplicationException
    {
        private IEnumerable<MetadataConversionError> importErrors = null;
        private IEnumerable<MetadataConversionError> codegenErrors = null;
        private IEnumerable<CompilerError> compilerErrors = null;

        public DynamicProxyException(string message)
            : base(message)
        {
        }

        public DynamicProxyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public IEnumerable<MetadataConversionError> MetadataImportErrors
        {
            get
            {
                return this.importErrors;
            }

            internal set
            {
                this.importErrors = value;
            }
        }

        public IEnumerable<MetadataConversionError> CodeGenerationErrors
        {
            get
            {
                return this.codegenErrors;
            }

            internal set
            {
                this.codegenErrors = value;
            }
        }

        public IEnumerable<CompilerError> CompilationErrors
        {
            get
            {
                return this.compilerErrors;
            }

            internal set
            {
                this.compilerErrors = value;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(base.ToString());

            if (MetadataImportErrors != null)
            {
                builder.AppendLine("Metadata Import Errors:");
                builder.AppendLine(DynamicProxyFactory.ToString(
                            MetadataImportErrors));
            }

            if (CodeGenerationErrors != null)
            {
                builder.AppendLine("Code Generation Errors:");
                builder.AppendLine(DynamicProxyFactory.ToString(
                            CodeGenerationErrors));
            }

            if (CompilationErrors != null)
            {
                builder.AppendLine("Compilation Errors:");
                builder.AppendLine(DynamicProxyFactory.ToString(
                            CompilationErrors));
            }

            return builder.ToString();
        }
    }
}
