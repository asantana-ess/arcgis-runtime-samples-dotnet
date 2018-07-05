// Copyright 2016 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using System;
using CoreGraphics;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI.Controls;
using Foundation;
using UIKit;

namespace ArcGISRuntime.Samples.RenderUniqueValues
{
    [Register("RenderUniqueValues")]
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Unique value renderer",
        "Symbology",
        "This sample demonstrate how to use a unique value renderer to style different features in a feature layer with different symbols. Features do not have a symbol property for you to set, renderers should be used to define the symbol for features in feature layers. The unique value renderer allows for separate symbols to be used for features that have specific attribute values in a defined field.",
        "")]
    public class RenderUniqueValues : UIViewController
    {
        // Create and hold a reference to the used MapView.
        private readonly MapView _myMapView = new MapView();

        public RenderUniqueValues()
        {
            Title = "Render unique values";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Create the UI, setup the control references and execute initialization.
            CreateLayout();
            Initialize();
        }

        public override void ViewDidLayoutSubviews()
        {
            try
            {
                nfloat topMargin = NavigationController.NavigationBar.Frame.Height + UIApplication.SharedApplication.StatusBarFrame.Height;

                // Reposition controls.
                _myMapView.Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);
                _myMapView.ViewInsets = new UIEdgeInsets(topMargin, 0, 0, 0);

                base.ViewDidLayoutSubviews();
            }
            // Needed to prevent crash when NavigationController is null. This happens sometimes when switching between samples.
            catch (NullReferenceException)
            {
            }
        }

        private void Initialize()
        {
            // Create new Map with a topographic basemap.
            Map myMap = new Map(Basemap.CreateTopographic());

            // Create URI pointing to the feature service.
            var serviceUri = new Uri("https://sampleserver6.arcgisonline.com/arcgis/rest/services/Census/MapServer/3");

            // Create service feature table.
            ServiceFeatureTable statesFeatureTable = new ServiceFeatureTable(serviceUri);

            // Create a new feature layer using the service feature table.
            FeatureLayer statesLayer = new FeatureLayer(statesFeatureTable);

            // Create a new unique value renderer.
            UniqueValueRenderer regionRenderer = new UniqueValueRenderer();

            // Add the "SUB_REGION" field to the renderer.
            regionRenderer.FieldNames.Add("SUB_REGION");

            // Define a line symbol to use for the region fill symbols.
            SimpleLineSymbol stateOutlineSymbol = new SimpleLineSymbol(
                SimpleLineSymbolStyle.Solid, System.Drawing.Color.White, 0.7);

            // Define distinct fill symbols for a few regions (use the same outline symbol).
            SimpleFillSymbol pacificFillSymbol = new SimpleFillSymbol(
                SimpleFillSymbolStyle.Solid, System.Drawing.Color.Blue, stateOutlineSymbol);
            SimpleFillSymbol mountainFillSymbol = new SimpleFillSymbol(
                SimpleFillSymbolStyle.Solid, System.Drawing.Color.LawnGreen, stateOutlineSymbol);
            SimpleFillSymbol westSouthCentralFillSymbol = new SimpleFillSymbol(
                SimpleFillSymbolStyle.Solid, System.Drawing.Color.SandyBrown, stateOutlineSymbol);

            // Add values to the renderer: define the label, description, symbol, and attribute value for each.
            regionRenderer.UniqueValues.Add(
                new UniqueValue("Pacific", "Pacific Region", pacificFillSymbol, "Pacific"));
            regionRenderer.UniqueValues.Add(
                new UniqueValue("Mountain", "Rocky Mountain Region", mountainFillSymbol, "Mountain"));
            regionRenderer.UniqueValues.Add(
                new UniqueValue("West South Central", "West South Central Region", westSouthCentralFillSymbol, "West South Central"));

            // Set the default region fill symbol (transparent with no outline) for regions not explicitly defined in the renderer
            regionRenderer.DefaultSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Cross, System.Drawing.Color.Gray, null);
            regionRenderer.DefaultLabel = "Other";

            // Apply the unique value renderer to the states layer.
            statesLayer.Renderer = regionRenderer;

            // Add created layer to the map.
            myMap.OperationalLayers.Add(statesLayer);

            // Assign the map to the MapView.
            _myMapView.Map = myMap;

            // Feature table initialization.
            statesFeatureTable.RetryLoadAsync();
        }

        private void CreateLayout()
        {
            // Add MapView to the page.
            View.AddSubviews(_myMapView);
        }
    }
}