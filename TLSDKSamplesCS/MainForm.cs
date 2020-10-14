﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Neodynamic.SDK.Printing;
using System.Data.SqlClient;
using System.IO;

namespace ThermalLabelSdkSamplesCS
{
    public partial class MainForm : Form
    {

        const string LABEL_NOTE_4x3 = "IMPORTANT: This sample label was designed considering a label size of 4\" x 3\" (width & height). Please change the source code provided with these samples to accommodate them to your own label size before continuing. Do you want to continue?";
        const string LABEL_NOTE_3x2 = "IMPORTANT: This sample label was designed considering a label size of 3\" x 2\" (width & height). Please change the source code provided with these samples to accommodate them to your own label size before continuing. Do you want to continue?";
        

        double _dpi = 96;
        ThermalLabel _currentThermalLabel = null;
        int _currentDemoIndex = -1;
        ImageSettings _imgSettings = new ImageSettings();


        public MainForm()
        {
            InitializeComponent();
        }

        



        private void MainForm_Load(object sender, EventArgs e)
        {
            

            this.lstDemos.SelectedIndex = 0;
            this.cboDpi.SelectedIndex = 0;


            

        }

        
        private void DisplayThermalLabel()
        {

            int copies = 1;
                                    
            if (_currentDemoIndex == 0)
            {
                //Basic ThermalLabel
                _currentThermalLabel = this.GenerateBasicThermalLabel();
            }
            else if (_currentDemoIndex == 1)
            {
                //Advanced ThermalLabel
                _currentThermalLabel = this.GenerateAdvancedThermalLabel();
            }
            else if (_currentDemoIndex == 2)
            {
                //Data Binding with ThermalLabel
                _currentThermalLabel = this.GenerateThermalLabelDataBinding();
            }
            else if (_currentDemoIndex == 3)
            {
                //Counters with ThermalLabel
                _currentThermalLabel = this.GenerateThermalLabelCounters();
                copies = 5; //set copies to the num of labels to generate with counters to 5... 
            }
            else if (_currentDemoIndex == 4)
            {
                //Data Masking with ThermalLabel
                _currentThermalLabel = this.GenerateThermalLabelDataMasking();
                copies = 5; //set copies to the num of labels to generate with data masking to 5...
            }
            else if (_currentDemoIndex == 5)
            {
                //Advanced Color ThermalLabel
                _currentThermalLabel = this.GenerateAdvancedColorThermalLabel();
            }

            //temp folder for holding thermal label images
            this.imgViewer.Clear();
            string myDir = Directory.GetCurrentDirectory() + @"\temp\";
            if (Directory.Exists(myDir) == false) Directory.CreateDirectory(myDir);
            DirectoryInfo di = new DirectoryInfo(myDir);
            foreach (FileInfo file in di.GetFiles()) file.Delete();
            
            
            if (_currentThermalLabel != null)
            {
                try
                {
                    //Create ThermalLabel images
                    using (PrintJob pj = new PrintJob())
                    {
                        pj.ThermalLabel = _currentThermalLabel;
                        pj.Copies = copies;

                        ImageSettings imgSett = new ImageSettings();
                        imgSett.ImageFormat = ImageFormat.Png;
                        imgSett.AntiAlias = true;
                        
                        pj.ExportToImage(myDir + "TL.png", imgSett, _dpi);

                        this.imgViewer.LoadImages(myDir);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void lstDemos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_currentDemoIndex != this.lstDemos.SelectedIndex)
            {
                _currentDemoIndex = this.lstDemos.SelectedIndex;
                this.DisplayThermalLabel();
            }
        
            //demo overview
            if (_currentDemoIndex == 0)
                lblDemoOverview.Text = "This is a simple 4in x 3in label featuring a TextItem and a BarcodeItem.";
            else if (_currentDemoIndex == 1)
                lblDemoOverview.Text = "This is an advanced 4in x 3in label featuring ImageItem, TextItem and BarcodeItem objects with border settings, text sizing and white text on black background.";
            else if (_currentDemoIndex == 2)
                lblDemoOverview.Text = "ThermalLabel SDK supports .NET Data Binding scenarios allowing you to print thermal labels bound to a data source such as custom .NET objects, XML files, Databases, ADO.NET, etc.\n\nThe following sample features a class called Product with two basic properties: Id and Name. A list of Product and a ThermalLabel objects are used to perform data binding scenario generating a set of thermal labels for each product.";
            else if (_currentDemoIndex == 3)
                lblDemoOverview.Text = "Counters allow you to index data items by a selected increment or decrement value, making the data items to increase or decrease by a specified value each time a label is printed. Counters can be used with TextItem as well as BarcodeItem objects.\n\nThe following sample features a Counter scenario generating 5 labels with a Barcode Code-128 for values ranging from \"ABC1\" to \"ABC5\" and a text decreasing from 50 to 46.";
            else if (_currentDemoIndex == 4)
                lblDemoOverview.Text = "Data Masking allows you to serialize data items by specifying a Mask string as well as an increment string. Data Masking can be used with TextItem as well as BarcodeItem.\n\nIn the following sample, each label features a TextItem which serializes a product model ranging from \"MDL-001/X\" to \"MDL-005/X\" (i.e. the sequence is MDL-001/X, MDL-002/X, MDL-003/X, ..., MDL-005/X) and a BarcodeItem which serializes a product ID ranging from \"PRD000-A\" to \"PRD400-E\" (i.e. the sequence is \"PRD000-A\", \"PRD100-B\", ..., \"PRD400-E\") in Code 128 symbology.";
            else if (_currentDemoIndex == 5)
                lblDemoOverview.Text = "This is an advanced 4in x 3in label featuring color ImageItem, TextItem and BarcodeItem objects with border settings, text sizing and white text on color background.";

        }

        private void cboDpi_SelectedIndexChanged(object sender, EventArgs e)
        {
            double tmpDPI = 96;
            
            if(cboDpi.SelectedItem.ToString() != "Screen")
                tmpDPI = double.Parse(cboDpi.SelectedItem.ToString());

            if (tmpDPI != _dpi)
            {
                _dpi = tmpDPI;
                this.DisplayThermalLabel();
            }
        }



        private ThermalLabel GenerateBasicThermalLabel()
        {


            //Define a ThermalLabel object and set unit to inch and label size
            ThermalLabel tLabel = new ThermalLabel(UnitType.Inch, 4, 3);
            tLabel.GapLength = 0.2;

            TextItem txtItem = new TextItem(0.2, 0.2, 2.5, 0.5, "Thermal Label Test");
            txtItem.Font.Name = "ZPL Font 0";
            txtItem.Font.Size = 12;

            //Define a BarcodeItem object (Linear 1D)
            BarcodeItem bcItem = new BarcodeItem(0.2, 1, 2, 2, BarcodeSymbology.Code128, "ABC123");
            //Set bars height to .75inch
            bcItem.BarHeight = 0.75;
            //Set bars width to 0.0104inch
            bcItem.BarWidth = 0.0104;

            //Define a BarcodeItem object (2D)
            BarcodeItem bcItem2D = new BarcodeItem(2.5, 1, 1, 1, BarcodeSymbology.QRCode, "Encoding Current Date: " + DateTime.Now.ToShortDateString());
            //Set symbol sizing
            bcItem2D.Sizing = BarcodeSizing.FitProportional;

            //Add items to ThermalLabel object...
            tLabel.Items.Add(txtItem);
            tLabel.Items.Add(bcItem);
            tLabel.Items.Add(bcItem2D);

            return tLabel;

    
        }


        private ThermalLabel GenerateAdvancedThermalLabel()
        {

            //Define a ThermalLabel object and set unit to inch and label size
            ThermalLabel tLabel = new ThermalLabel(UnitType.Inch, 4, 3);
            tLabel.GapLength = 0.2;

            //Define an ImageItem for AdventureWorks logo
            ImageItem awLogo = new ImageItem(0.1, 0.1);
            awLogo.SourceBase64 = _adventureWorksLogo;
            awLogo.Width = 1.5;
            awLogo.LockAspectRatio = LockAspectRatio.WidthBased;
            awLogo.MonochromeSettings.DitherMethod = DitherMethod.Threshold;
            awLogo.MonochromeSettings.Threshold = 80;

            //Define a TextItem for 'AW' id
            TextItem txtAW = new TextItem(2.8, 0.1, 1.1, 0.5, "AW");
            //font settings
            txtAW.Font.Name = "Arial";
            txtAW.Font.Bold = true;
            //stretch text
            txtAW.Sizing = TextSizing.Stretch;
            //border settings
            txtAW.BorderThickness = new FrameThickness(0.02);
            txtAW.CornerRadius = new RectangleCornerRadius(0.05);
            txtAW.TextPadding = new FrameThickness(0);

            //Define a TextItem for 'Model Name'
            TextItem txtModelName = new TextItem(0.1, 0.75, 3.8, 0.25, "Model Name: ROAD 150");
            //font settings
            txtModelName.Font.Name = "Arial";
            txtModelName.Font.Unit = FontUnit.Point;
            txtModelName.Font.Size = 12;
            //white text on black background
            txtModelName.BackColor = Neodynamic.SDK.Printing.Color.Black;
            txtModelName.ForeColor = Neodynamic.SDK.Printing.Color.White;
            //padding
            txtModelName.TextPadding = new FrameThickness(0.075, 0.03, 0, 0);

            //Define a TextItem for 'Model Code' (random code)
            TextItem txtModelCode = new TextItem(0.1, 1, 3.8, 0.25, "Model Code: " + Guid.NewGuid().ToString().ToUpper().Substring(0, 17));
            //font settings
            txtModelCode.Font.Name = "Arial";
            txtModelCode.Font.Unit = FontUnit.Point;
            txtModelCode.Font.Size = 12;
            //white text on black background
            txtModelCode.BackColor = Neodynamic.SDK.Printing.Color.Black;
            txtModelCode.ForeColor = Neodynamic.SDK.Printing.Color.White;
            //padding
            txtModelCode.TextPadding = new FrameThickness(0.075, 0.03, 0, 0);

            //Define a BarcodeItem for a random 'Serial Number'
            string serialNum = Guid.NewGuid().ToString().ToUpper().Substring(0, 7);
            BarcodeItem serialBarcode = new BarcodeItem(0.1, 1.25, 3.8, 0.5, BarcodeSymbology.Code39, serialNum);
            //Set bars height to .3inch
            serialBarcode.BarHeight = 0.3;
            //Set bars width to 0.02inch
            serialBarcode.BarWidth = 0.02;
            //disable checksum
            serialBarcode.AddChecksum = false;
            //hide human readable text
            serialBarcode.DisplayCode = false;
            //set border
            serialBarcode.BorderThickness = new FrameThickness(0.02);
            //align barcode
            serialBarcode.BarcodeAlignment = BarcodeAlignment.MiddleCenter;


            //Define a TextItem for 'Serial Num'
            TextItem txtSN = new TextItem(0.1, 1.75 - serialBarcode.BorderThickness.Bottom, 1.25, 0.3, "S/N: " + serialNum);
            //font settings
            txtSN.Font.Name = "Arial";
            txtSN.Font.Bold = true;
            txtSN.Font.Unit = FontUnit.Point;
            txtSN.Font.Size = 12;
            //padding
            txtSN.TextPadding = new FrameThickness(0.03);
            //set border
            txtSN.BorderThickness = new FrameThickness(0.02);
            txtSN.TextAlignment = TextAlignment.Center;

            //Define a TextItem for legend
            TextItem txtLegend = new TextItem(txtSN.X + txtSN.Width - txtSN.BorderThickness.Right, txtSN.Y, 3.8 - txtSN.Width + txtSN.BorderThickness.Right, txtSN.Height, "This bike is ridden by race winners! Brought to you by Adventure Works Cycles professional race team.");
            //font settings
            txtLegend.Font.Name = "Arial";
            txtLegend.Font.Unit = FontUnit.Point;
            txtLegend.Font.Size = 7.5;
            //padding
            txtLegend.TextPadding = new FrameThickness(0.03, 0, 0, 0);
            //set border
            txtLegend.BorderThickness = new FrameThickness(0.02);

            //Define another BarcodeItem for EAN-13 symbology
            BarcodeItem eanBarcode = new BarcodeItem(0.1, 2.1, 3, 0.9, BarcodeSymbology.Ean13, "0729507704739");
            //Set barcode dimensions...
            eanBarcode.BarHeight = 0.5;
            eanBarcode.BarWidth = 0.02;
            eanBarcode.EanUpcGuardBarHeight = 0.55;
            //human readable text font settings
            eanBarcode.Font.Name = Neodynamic.SDK.Printing.Font.NativePrinterFontA;
            eanBarcode.Font.Unit = FontUnit.Point;
            eanBarcode.Font.Size = 5;
            //set human readable text align
            eanBarcode.CodeAlignment = BarcodeTextAlignment.BelowJustify;

            //Define an ImageItem for NBDA logo
            ImageItem nbdaLogo = new ImageItem(2.9, 2.1);
            nbdaLogo.SourceBase64 = _nbdaLogo;
            nbdaLogo.Width = 1;
            nbdaLogo.LockAspectRatio = LockAspectRatio.WidthBased;
            nbdaLogo.MonochromeSettings.DitherMethod = DitherMethod.Threshold;
            nbdaLogo.MonochromeSettings.Threshold = 50;
            nbdaLogo.MonochromeSettings.ReverseEffect = true;

            //Define a LineShapeItem
            LineShapeItem line = new LineShapeItem(0.1, 2.8, 3.8, 0.03);
            line.Orientation = LineOrientation.Horizontal;
            line.StrokeThickness = 0.03;


            //Add items to ThermalLabel object...
            tLabel.Items.Add(awLogo);
            tLabel.Items.Add(txtAW);
            tLabel.Items.Add(txtModelName);
            tLabel.Items.Add(txtModelCode);
            tLabel.Items.Add(serialBarcode);
            tLabel.Items.Add(txtSN);
            tLabel.Items.Add(txtLegend);
            tLabel.Items.Add(eanBarcode);
            tLabel.Items.Add(nbdaLogo);
            tLabel.Items.Add(line);

            return tLabel;
        }

        private ThermalLabel GenerateAdvancedColorThermalLabel()
        {

            //Define a ThermalLabel object and set unit to inch and label size
            ThermalLabel tLabel = new ThermalLabel(UnitType.Inch, 4, 3);
            tLabel.GapLength = 0.2;

            //Define an ImageItem for AdventureWorks logo
            ImageItem awLogo = new ImageItem(0.1, 0.1);
            awLogo.SourceBase64 = _adventureWorksLogo;
            awLogo.Width = 1.5;
            awLogo.LockAspectRatio = LockAspectRatio.WidthBased;
            awLogo.ConvertToMonochrome = false; //enable Color image

            //Define a TextItem for 'AW' id
            TextItem txtAW = new TextItem(2.8, 0.1, 1.1, 0.5, "AW");
            //font settings
            txtAW.Font.Name = "Arial";
            txtAW.Font.Bold = true;
            //fore color
            txtAW.ForeColorHex = "#ff8040";
            //stretch text
            txtAW.Sizing = TextSizing.Stretch;
            //border settings
            txtAW.BorderThickness = new FrameThickness(0.02);
            txtAW.CornerRadius = new RectangleCornerRadius(0.05);
            txtAW.TextPadding = new FrameThickness(0);
            txtAW.BorderColorHex = "#ff8040";

            //Define a TextItem for 'Model Name'
            TextItem txtModelName = new TextItem(0.1, 0.75, 3.8, 0.25, "Model Name: ROAD 150");
            //font settings
            txtModelName.Font.Name = "Arial";
            txtModelName.Font.Unit = FontUnit.Point;
            txtModelName.Font.Size = 12;
            //white text on black background
            txtModelName.BackColor = Neodynamic.SDK.Printing.Color.Black;
            txtModelName.BackColorHex = "#20A9DD";
            txtModelName.ForeColor = Neodynamic.SDK.Printing.Color.White;
            //padding
            txtModelName.TextPadding = new FrameThickness(0.075, 0.03, 0, 0);

            //Define a TextItem for 'Model Code' (random code)
            TextItem txtModelCode = new TextItem(0.1, 1, 3.8, 0.25, "Model Code: " + Guid.NewGuid().ToString().ToUpper().Substring(0, 17));
            //font settings
            txtModelCode.Font.Name = "Arial";
            txtModelCode.Font.Unit = FontUnit.Point;
            txtModelCode.Font.Size = 12;
            //white text on black background
            txtModelCode.BackColor = Neodynamic.SDK.Printing.Color.Black;
            txtModelCode.BackColorHex = "#20A9DD";
            txtModelCode.ForeColor = Neodynamic.SDK.Printing.Color.White;
            //padding
            txtModelCode.TextPadding = new FrameThickness(0.075, 0.03, 0, 0);

            //Define a BarcodeItem for a random 'Serial Number'
            string serialNum = Guid.NewGuid().ToString().ToUpper().Substring(0, 7);
            BarcodeItem serialBarcode = new BarcodeItem(0.1, 1.25, 3.8, 0.5, BarcodeSymbology.Code39, serialNum);
            //Set bars height to .3inch
            serialBarcode.BarHeight = 0.3;
            //Set bars width to 0.02inch
            serialBarcode.BarWidth = 0.02;
            //disable checksum
            serialBarcode.AddChecksum = false;
            //hide human readable text
            serialBarcode.DisplayCode = false;
            //set border
            serialBarcode.BorderThickness = new FrameThickness(0.02);
            serialBarcode.BorderColorHex = "#20A9DD";
            //align barcode
            serialBarcode.BarcodeAlignment = BarcodeAlignment.MiddleCenter;


            //Define a TextItem for 'Serial Num'
            TextItem txtSN = new TextItem(0.1, 1.75 - serialBarcode.BorderThickness.Bottom, 1.25, 0.3, "S/N: " + serialNum);
            //font settings
            txtSN.Font.Name = "Arial";
            txtSN.Font.Bold = true;
            txtSN.Font.Unit = FontUnit.Point;
            txtSN.Font.Size = 12;
            //padding
            txtSN.TextPadding = new FrameThickness(0.03);
            //set border
            txtSN.BorderThickness = new FrameThickness(0.02);
            txtSN.BorderColorHex = "#20A9DD";
            txtSN.TextAlignment = TextAlignment.Center;

            //Define a TextItem for legend
            TextItem txtLegend = new TextItem(txtSN.X + txtSN.Width - txtSN.BorderThickness.Right, txtSN.Y, 3.8 - txtSN.Width + txtSN.BorderThickness.Right, txtSN.Height, "This bike is ridden by race winners! Brought to you by Adventure Works Cycles professional race team.");
            //font settings
            txtLegend.Font.Name = "Arial";
            txtLegend.Font.Unit = FontUnit.Point;
            txtLegend.Font.Size = 7.5;
            //padding
            txtLegend.TextPadding = new FrameThickness(0.03, 0, 0, 0);
            //set border
            txtLegend.BorderThickness = new FrameThickness(0.02);
            txtLegend.BorderColorHex = "#20A9DD";

            //Define another BarcodeItem for EAN-13 symbology
            BarcodeItem eanBarcode = new BarcodeItem(0.1, 2.1, 3, 0.9, BarcodeSymbology.Ean13, "0729507704739");
            //Set barcode dimensions...
            eanBarcode.BarHeight = 0.5;
            eanBarcode.BarWidth = 0.02;
            eanBarcode.EanUpcGuardBarHeight = 0.55;
            //human readable text font settings
            eanBarcode.Font.Name = Neodynamic.SDK.Printing.Font.NativePrinterFontA;
            eanBarcode.Font.Unit = FontUnit.Point;
            eanBarcode.Font.Size = 5;
            //set human readable text align
            eanBarcode.CodeAlignment = BarcodeTextAlignment.BelowJustify;

            //Define an ImageItem for NBDA logo
            ImageItem nbdaLogo = new ImageItem(2.9, 2.1);
            nbdaLogo.SourceBase64 = _nbdaLogo;
            nbdaLogo.Width = 1;
            nbdaLogo.LockAspectRatio = LockAspectRatio.WidthBased;
            nbdaLogo.ConvertToMonochrome = false; //enable color image

            //Define a LineShapeItem
            LineShapeItem line = new LineShapeItem(0.1, 2.8, 3.8, 0.03);
            line.Orientation = LineOrientation.Horizontal;
            line.StrokeThickness = 0.03;
            line.StrokeColorHex = "#ff8040";

            //Add items to ThermalLabel object...
            tLabel.Items.Add(awLogo);
            tLabel.Items.Add(txtAW);
            tLabel.Items.Add(txtModelName);
            tLabel.Items.Add(txtModelCode);
            tLabel.Items.Add(serialBarcode);
            tLabel.Items.Add(txtSN);
            tLabel.Items.Add(txtLegend);
            tLabel.Items.Add(eanBarcode);
            tLabel.Items.Add(nbdaLogo);
            tLabel.Items.Add(line);

            return tLabel;
        }


        private ThermalLabel GenerateThermalLabelDataBinding()
        {
            //Define a ThermalLabel object and set unit to inch and label size
            ThermalLabel tLabel = new ThermalLabel(UnitType.Inch, 3, 2);
            tLabel.GapLength = 0.2;

            //Define a TextItem object for product name
            TextItem txt = new TextItem(0.1, 0.1, 2.8, 0.5, "");
            //set data field
            txt.DataField = "Name";
            txt.Expression = "[DataFields!Name]";
            //set font
            //txt.Font.Name = Neodynamic.SDK.Printing.Font.NativePrinterFontA;
            txt.Font.Name = "ZPL Font 0";
            txt.Font.Unit = FontUnit.Point;
            txt.Font.Size = 10;
            //set border
            txt.BorderThickness = new FrameThickness(0.03);

            //set alignment
            txt.TextAlignment = TextAlignment.Center;
            txt.TextPadding = new FrameThickness(0, 0.1, 0, 0);

            //Define a BarcodeItem object for encoding product id with a Code 128 symbology
            BarcodeItem bc = new BarcodeItem(0.1, 0.57, 2.8, 1.3, BarcodeSymbology.Code128, "");
            //set data field
            bc.DataField = "Id";
            //set barcode size
            bc.BarWidth = 0.01;
            bc.BarHeight = 0.75;
            //set barcode alignment
            bc.BarcodeAlignment = BarcodeAlignment.MiddleCenter;
            //set text alignment
            bc.CodeAlignment = BarcodeTextAlignment.BelowCenter;
            //set border
            bc.BorderThickness = new FrameThickness(0.03);

            //Add items to ThermalLabel object...
            tLabel.Items.Add(txt);
            tLabel.Items.Add(bc);
            
            //Create data source...
            List<Product> products = new List<Product>();
            products.Add(new Product("OO2935", "Olive Oil Extra Virgen"));
            products.Add(new Product("CS4948", "Curry Sauce"));
            products.Add(new Product("CH0094", "Chocolate"));
            products.Add(new Product("MZ1027", "Mozzarella"));

            //set data source...
            tLabel.DataSource = products;
            
            return tLabel;

        }


        private ThermalLabel GenerateThermalLabelCounters()
        {
            //Define a ThermalLabel object and set unit to inch and label size
            ThermalLabel tLabel = new ThermalLabel(UnitType.Inch, 3, 2);
            tLabel.GapLength = 0.2;

            //Define a TextItem object 
            TextItem txt = new TextItem(0.1, 0.1, 2.8, 0.5, "Decreasing 50");
            //set counter step for decreasing by 1
            txt.CounterStep = -1;
            //set font
            txt.Font.Name = Neodynamic.SDK.Printing.Font.NativePrinterFontA;
            txt.Font.Unit = FontUnit.Point;
            txt.Font.Size = 10;

            //Define a BarcodeItem object
            BarcodeItem bc = new BarcodeItem(0.1, 0.57, 2.8, 1.3, BarcodeSymbology.Code128, "ABC01");
            //set counter step for increasing by 1
            bc.CounterStep = 1;
            bc.CounterUseLeadingZeros = true;
            //set barcode size
            bc.BarWidth = 0.02;
            bc.BarHeight = 0.75;
            //set barcode alignment
            bc.BarcodeAlignment = BarcodeAlignment.MiddleCenter;
            //set font
            bc.Font.Name = Neodynamic.SDK.Printing.Font.NativePrinterFontA;
            bc.Font.Unit = FontUnit.Point;
            bc.Font.Size = 10;

            //Add items to ThermalLabel object...
            tLabel.Items.Add(txt);
            tLabel.Items.Add(bc);
            
            return tLabel;
        }


        private ThermalLabel GenerateThermalLabelDataMasking()
        {
            //Define a ThermalLabel object and set unit to inch and label size
            ThermalLabel tLabel = new ThermalLabel(UnitType.Inch, 3, 2);
            tLabel.GapLength = 0.2;

            //Define a TextItem object 
            TextItem txt = new TextItem(0.1, 0.1, 2.8, 0.5, "MDL-001/X");
            //set Mask info...
            txt.Mask = "%%%%ddd%%";
            txt.MaskIncrement = "1%%";
            //set font
            txt.Font.Name = Neodynamic.SDK.Printing.Font.NativePrinterFontA;
            txt.Font.Unit = FontUnit.Point;
            txt.Font.Size = 10;

            //Define a BarcodeItem object
            BarcodeItem bc = new BarcodeItem(0.1, 0.57, 2.8, 1.3, BarcodeSymbology.Code128, "PRD000-A");
            //set Mask info...
            bc.Mask = "%%%d%%%A";
            bc.MaskIncrement = "1%%%B";
            //set barcode size
            bc.BarWidth = 0.02;
            bc.BarHeight = 0.75;
            //set barcode alignment
            bc.BarcodeAlignment = BarcodeAlignment.MiddleCenter;
            //set font
            bc.Font.Name = Neodynamic.SDK.Printing.Font.NativePrinterFontA;
            bc.Font.Unit = FontUnit.Point;
            bc.Font.Size = 10;

            //Add items to ThermalLabel object...
            tLabel.Items.Add(txt);
            tLabel.Items.Add(bc);

            return tLabel;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

            //warning about label size
            string msg = LABEL_NOTE_3x2;
            if (_currentDemoIndex == 0 ||
                _currentDemoIndex == 1 ||
                _currentDemoIndex == 5)
            {
                msg = LABEL_NOTE_4x3;
            }

            if (MessageBox.Show(msg, "NOTE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }


            //Display Print Job dialog...           
            PrintJobDialog frmPrintJob = new PrintJobDialog();
            if (frmPrintJob.ShowDialog() == DialogResult.OK)
            {

                //create a PrintJob object
                using (WindowsPrintJob pj = new WindowsPrintJob(frmPrintJob.PrinterSettings))
                {
                    pj.Copies = frmPrintJob.Copies; // set copies
                    pj.PrintOrientation = frmPrintJob.PrintOrientation; //set orientation
                    pj.ThermalLabel = _currentThermalLabel; // set the ThermalLabel object

                    if (frmPrintJob.PrintAsGraphic)
                        pj.PrintAsGraphic(); //print to any printer using Windows driver
                    else
                        pj.Print(); //print to thermal printer                       
                }
            }
        }

        private void btnExportToPdf_Click(object sender, EventArgs e)
        {

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Adobe PDF|*.pdf";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //create a PrintJob object
                using (PrintJob pj = new PrintJob())
                {
                    pj.ThermalLabel = _currentThermalLabel; // set the ThermalLabel object

                    //set num of copies if Counters or Data Masking demos
                    if (_currentDemoIndex == 3 || _currentDemoIndex == 4)
                        pj.Copies = 5;

                    pj.ExportToPdf(sfd.FileName, _dpi); //export to pdf
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }

        private void btnXmlTemplate_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XML Template|*.tl";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //save ThermalLabel to XML template
                System.IO.File.WriteAllText(sfd.FileName, _currentThermalLabel.GetXmlTemplate());

                if (MessageBox.Show("XML Template saved! Do you want to open it?", "ThermalLabel SDK", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(sfd.FileName);
                } 
                
            }
        }

        private void btnToImagePng_Click(object sender, EventArgs e)
        {
            _imgSettings.ImageFormat = ImageFormat.Png;
            this.ExportToImage();
        }

        private void btnToImageJpeg_Click(object sender, EventArgs e)
        {
            _imgSettings.ImageFormat = ImageFormat.Jpeg;
            this.ExportToImage();
        }

        private void btnToImageSvg_Click(object sender, EventArgs e)
        {
            _imgSettings.ImageFormat = ImageFormat.Svg;
            this.ExportToImage();
        }

        
        private void ExportToImage()
        {
            SaveFileDialog sfd = new SaveFileDialog();

            if (_imgSettings.ImageFormat == ImageFormat.Png)
                sfd.Filter = "PNG|*.png";
            else if (_imgSettings.ImageFormat == ImageFormat.Jpeg)
                sfd.Filter = "JPEG|*.jpg";
            else if (_imgSettings.ImageFormat == ImageFormat.Pcx)
                sfd.Filter = "PCX|*.pcx";
            else if (_imgSettings.ImageFormat == ImageFormat.Svg)
            {
                MessageBox.Show("SVG output format is NOT available in TRIAL mode! Request a 30-days Product Key at https://neodynamic.com/support","SVG INFO", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                sfd.Filter = "SVG|*.svg";
            }
            
            
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //create a PrintJob object
                using (PrintJob pj = new PrintJob())
                {
                    pj.ThermalLabel = _currentThermalLabel; // set the ThermalLabel object

                    //set num of copies if Counters or Data Masking demos
                    if (_currentDemoIndex == 3 || _currentDemoIndex == 4)
                        pj.Copies = 5;

                    

                    pj.ExportToImage(sfd.FileName, _imgSettings, _dpi); //export to image file

                    
                    //Open folder where image file was created
                    System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(sfd.FileName));
                    
                }
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {

            System.Diagnostics.Process.Start("https://neodynamic.com/products/help/");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.neodynamic.com/Support");
        }

        private void btnToImagePcx_Click(object sender, EventArgs e)
        {
            _imgSettings.ImageFormat = ImageFormat.Pcx;
            this.ExportToImage();
        }


        const string _adventureWorksLogo = "/9j/4AAQSkZJRgABAQEAgACAAAD/2wBDAAoHBwgHBgoICAgLCgoLDhgQDg0NDh0VFhEYIx8lJCIfIiEmKzcvJik0KSEiMEExNDk7Pj4+JS5ESUM8SDc9Pjv/2wBDAQoLCw4NDhwQEBw7KCIoOzs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozv/wAARCAB0AWcDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD2aiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACmuyohZiAqjJJ7CnVi+L0un8I6stkxW4NrJ5ZGc5x/8AroA4/XPjboelX8lpaWk9/wCWxVpEYIuQcHGetdL4R8caV4xtXksGaOaL/WwS43p7+4r5aIIJByMV1XgO71fTrzUdR0g7XtLCR3Zl3KBkcEdP/wBdAH1BSV8/QfHLxREf3sFjMPeMjP5Gr8Hx91NSPP0a1cf7EjD+dAHudFeOwfH6A48/QZB6lJwf6Vrab8cfD95dx29xZ3dqJG2+Y21lH1xQB6ZRTVIZQQQQRkEd6dQAlFFRNcwpKImlQOeilhmhICaikpaACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAppAIIIyDwRTqSgDz3xF8JPCt/cTanI02ngAyTeSwCH1OCOPwrE8Jav4MvrPUfCHh62ntZr2GRI7i5AJuDjue3rirXxq8W/2fpa+HrSQCe9G64weVi7D8T+g96xvgh4Taa5l8S3SHZFmK1BGNxP3j/T8aAMhvgb4pB4msT/ANtT/hWRr3w01rw1YPeanc2EUa/dHn5Zz6KMc19NdK+evjLpmrweLJL67MkljMB9mfOVQY5X2OaAPPApdgqruJOAAMnNeneBPhFfam8Op66HtLMYdbfGJJR1GfQVU+GGo+EtOeSfUrRptYU/6MJACjegUdm+v4V60ln4i1zEt1dHTYCfliiHz/jWkYc2rdkB06NFEixhlUKMAZ6CnBgeRyK5weCLEqPMu7x3H8RkrV0jSl0m2aBJ5ZlL7gZDkgY6UpRgl7shal89K8k8Qzzf8JJeSGRvMjlO0g9MdMV6nfXkVjZy3MzAJGCT7+1eXabZT+Ite6HbJJvlbso6muzBJLmnLYUux6hpsrz6bbSyZDPErHPqRVqol2W8IUEKqKB6YFYWp+NtG00lDObiUfwRfMfxPQV506kYu7djWnSnUdoq50OaM15tefEy7ditjZIgPQyZY/pWZJ418TTEkSlB/sQ/4iuV4ymttTvhldd/FZHruaM143/wmPiOM/NfSf8AAkH+FW7X4h65ER5jQzL3DJ/UVP16n1TRbymva8WmesZpa4fTPiVZzFV1C3e3JON6Hcv/ANauusr+2v4BNazpNGehU10wqwn8LOGrhqtH44lqigGitTAWiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooASiis/WdUi0jT5LqTkjhFHVmPQU0m3ZALqes2ekw+Zdyhc/dQcs30Fc3qXi3VV06fULbThb2cEZkaa4bBIHt6mrei6E13KNY1n99cyfOkbDKxjtxVnxh4bHinw3caSLg25k2lXAJAIOeR6Vq3CGiV2JXPnG4m1Xx14sMhQz3l9JgKo+6uO3sAK968P6xbeHNPtNEvdOl09YECK5GUY9zn1J5rP+HXwzPhG5n1C/niuLx12RmMHbGvf8TXd3dlb31s0F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ACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigBK858afCHTfEVzJqGlyrp985LSDbmKVj3I/hJ9R+Vej0lAHzhd/BrxpbvtisYLsf3oblAP/HytWNN+CXiy7cfbBaWCfxGWXew+gTIP519D4oxQByXgn4fab4KhdreR7m8mULLcOMbh6BR0Ga6e7t4byzltrmNZIZUKSI3RlIwQamIHpXnnxd8ZHw/oP9mWcm2/1FSuQcGKLozfU9B+J7UAeF6zY2sHiO7sdKla7tluWjt2A5cZwMevp719F/DvwknhHwzFbyIovrgebduP73936KMD8680+DHg3+0dTPiK9j/0aybbbhh96bruH+7/ADI9K93HSgAwK858afCHTfEVzJqOmSrp985LSDbmOZjzkj+En1H5V6NRQB84Xfwa8aW77YrGC7H96G5QD/x8rVjTfgl4su3H2wWlgn8Rll3sPoEyD+dfQ+KMUAcf4S+HmneD9PuFtpHub25i8uW4fjIx0AH3Rn615h/wobxT/wA/+kf9/pf/AI3Xv+KMUAcP8MfBepeC9MvbXUprWV7iYOht3ZgAFxzlRVzxl8P9J8ZQq9zm2vkG2K6iHzAejDPzDvjr1wRmusxRQB886l8EvFVpI32P7LfR/wAJjlCE/UNgCoLb4Q+ObxhHcWkdui9DPdowH4KTX0bRQB4o3wLvodH2w31rNqUjDLyO8cUK99uFJdjjHOBgng8Ea/w8+FFz4Z11tV1qa0nkhTFskBZgrHq53KOQOn1PpXqmKMUAc74203XNX8OXFhoU9vFPcAxy/aM4aJlIZQQDhjkYOK8Tt/g14xmvRDNaQQRFsG4a4QrjvgA5/QV9HYFGKAMXwn4ctvCnh+30m2YyeX80kpGPMc/eb/PatodKKWgAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooASjFFFABtFLRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAFFFFABRRRQAUUUUAf//Z";

        const string _nbdaLogo = "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";
    }
}
