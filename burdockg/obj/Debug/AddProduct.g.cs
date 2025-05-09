﻿#pragma checksum "..\..\AddProduct.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "CF9F84AC1469D305545EB8E3831F3296CAD2CC0E5C07802AEEC4E7C522A67D31"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using burdockg;


namespace burdockg {
    
    
    /// <summary>
    /// AddProduct
    /// </summary>
    public partial class AddProduct : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 39 "..\..\AddProduct.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image productImage;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\AddProduct.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox productNameTextBox;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\AddProduct.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox productTypeComboBox;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\AddProduct.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox materialsListBox;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\AddProduct.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox materialsComboBox;
        
        #line default
        #line hidden
        
        
        #line 117 "..\..\AddProduct.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ArkTextBox;
        
        #line default
        #line hidden
        
        
        #line 125 "..\..\AddProduct.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox costTextBox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/burdockg;component/addproduct.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\AddProduct.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.productImage = ((System.Windows.Controls.Image)(target));
            return;
            case 2:
            
            #line 42 "..\..\AddProduct.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ChangeImageButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.productNameTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.productTypeComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 5:
            this.materialsListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 6:
            
            #line 88 "..\..\AddProduct.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AddMaterial_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 96 "..\..\AddProduct.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.RemoveMaterial_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.materialsComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 9:
            this.ArkTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.costTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 11:
            
            #line 133 "..\..\AddProduct.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.BackButton_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            
            #line 141 "..\..\AddProduct.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SaveButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

