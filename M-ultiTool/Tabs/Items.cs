﻿using MultiTool.Core;
using MultiTool.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MultiTool.Modules;
using MultiTool.Utilities.UI;
using MultiTool.Extensions;

namespace MultiTool.Tabs
{
	internal class ItemsTab : Tab
	{
		public override string Name => "Items";
		public override bool HasConfigPane => true;

        private Settings _settings = new Settings();

        // Scroll vectors.
        private Vector2 _itemScrollPosition;
        private Vector2 _configScrollPosition;
        private Vector2 _filterScrollPosition;

        // Main tab variables.
		private bool _filterShow = false;
		private List<int> _filters = new List<int>();
        private string _search = string.Empty;
        private string _lastSearch = string.Empty;
        private float _lastWidth = 0;
        private List<List<Item>> _itemsChunked = new List<List<Item>>();
        private bool _rechunk = false;

        // Config variables.
        private int _maxFuelType = 0;
        private int _maxCondition = 0;
        private int _condition = 0;
        private int _fuelMixes = 1;
        private List<float> _fuelValues = new List<float> { -1f };
        private List<int> _fuelTypes = new List<int> { -1 };
        private string _plate = string.Empty;

        public override void OnRegister()
        {
            _maxFuelType = (int)Enum.GetValues(typeof(mainscript.fluidenum)).Cast<mainscript.fluidenum>().Max();
            _maxCondition = (int)Enum.GetValues(typeof(Item.Condition)).Cast<Item.Condition>().Max();
        }

        public override void OnUnregister()
        {
            _fuelValues.Clear();
            _fuelTypes.Clear();
        }

        public override void RenderTab(Rect dimensions)
		{
            List<Item> items = GUIRenderer.items;
            if (_search != _lastSearch)
            {
                items = GUIRenderer.items.Where(i => i.gameObject.name.ToLower().Contains(_search.ToLower())).ToList();
                _rechunk = true;
                _lastSearch = _search;
                _itemScrollPosition = new Vector2(0, 0);
            }

            if (_filters.Count > 0)
            {
                items = items.Where(v => _filters.Contains(v.category)).ToList();
                _rechunk = true;
                _itemScrollPosition = new Vector2(0, 0);
            }

            float width = dimensions.width;
            if (_filterShow)
                width -= 200f;

            if (_lastWidth != width || _rechunk)
            {
                int rowLength = Mathf.FloorToInt(width / 150f);
                width = rowLength * 150f;
                _itemsChunked = items.ChunkBy(rowLength);
                _lastWidth = width;

                _rechunk = false;
            }

            GUILayout.BeginArea(dimensions);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.MaxWidth(50));
            GUILayout.Space(5);
            _search = GUILayout.TextField(_search, GUILayout.MaxWidth(500));
            GUILayout.Space(5);
            if (GUILayout.Button("Reset", GUILayout.MaxWidth(70)))
            {
                _search = string.Empty;
                _lastSearch = string.Empty;
                _rechunk = true;
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Filters", GUILayout.Width(200)))
            {
                _filterShow = !_filterShow;
                _rechunk = true;
            }

            //if (_filterShow)
            //{
            //    for (int i = 0; i < GUIRenderer.categories.Count; i++)
            //    {
            //        string name = GUIRenderer.categories.ElementAt(i).Key;
            //        if (GUILayout.Button(Accessibility.GetAccessibleString(name, _filters.Contains(i))))
            //        {
            //            if (_filters.Contains(i))
            //                _filters.Remove(i);
            //            else
            //                _filters.Add(i);

            //            _itemScrollPosition = new Vector2(0, 0);
            //        }
            //    }
            //}
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.MaxWidth(_lastWidth));
            _itemScrollPosition = GUILayout.BeginScrollView(_itemScrollPosition);
            foreach (List<Item> itemsRow in _itemsChunked)
            {
                GUILayout.BeginHorizontal();
                foreach (Item item in itemsRow)
                {
                    GUILayout.Box("", "button", GUILayout.Width(140), GUILayout.Height(140));
                    Rect boxRect = GUILayoutUtility.GetLastRect();
                    bool buttonImage = GUI.Button(new Rect(boxRect.x + 10f, boxRect.y - 10f, boxRect.width - 20f, boxRect.height - 20f), item.thumbnail, "ButtonTransparent");
                    bool buttonText = GUI.Button(new Rect(boxRect.x, boxRect.y + (boxRect.height / 2), boxRect.width, boxRect.height / 2), item.gameObject.name, "ButtonTransparent");
                    if (buttonImage || buttonText)
                    {
                        SpawnUtilities.Spawn(new Item()
                        {
                            gameObject = item.gameObject,
                            conditionInt = _condition,
                            fuelMixes = _fuelMixes,
                            fuelValues = _fuelValues,
                            fuelTypeInts = _fuelTypes,
                            color = Colour.GetColour(),
                            amt = item.amt,
                        });
                    }
                    GUILayout.Space(5);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            if (_filterShow)
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical(GUILayout.MaxWidth(205));
                _filterScrollPosition = GUILayout.BeginScrollView(_filterScrollPosition);
                for (int i = 0; i < GUIRenderer.categories.Count; i++)
                {
                    string name = GUIRenderer.categories.ElementAt(i).Key;
                    if (GUILayout.Button(Accessibility.GetAccessibleString(name, _filters.Contains(i))))
                    {
                        if (_filters.Contains(i))
                            _filters.Remove(i);
                        else
                            _filters.Add(i);
                        _rechunk = true;

                        _itemScrollPosition = new Vector2(0, 0);
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndArea();
		}

        public override void RenderConfigPane(Rect dimensions)
        {
            GUILayout.BeginArea(dimensions);
            GUILayout.BeginVertical();
            _configScrollPosition = GUILayout.BeginScrollView(_configScrollPosition);

            // Condition.
            GUILayout.Label($"Condition: {(Item.Condition)_condition}");
            _condition = Mathf.RoundToInt(GUILayout.HorizontalSlider(_condition, -1, _maxCondition));
            GUILayout.Space(10);

            // Plate.
            // TODO: Add support for setting item plate text.
            //GUILayout.Label("Plate (blank for random):");
            //_plate = GUILayout.TextField(_plate);
            //GUILayout.Space(10);

            // Spawn with fuel.
            if (GUILayout.Button(Accessibility.GetAccessibleString("Spawn with fuel", _settings.spawnWithFuel)))
                _settings.spawnWithFuel = !_settings.spawnWithFuel;
            GUILayout.Space(10);

            // Fuel mixes.
            for (int i = 0; i < _fuelMixes; i++)
            {
                GUILayout.BeginVertical($"Fluid {i + 1}", "box");
                GUILayout.Space(10);

                // Fluid type.
                string fuelType = ((mainscript.fluidenum)_fuelTypes[i]).ToString();
                if (_fuelTypes[i] == -1)
                    fuelType = "Default";
                else
                    fuelType = fuelType[0].ToString().ToUpper() + fuelType.Substring(1);
                GUILayout.Label($"Fluid type: {fuelType}");
                _fuelTypes[i] = Mathf.RoundToInt(GUILayout.HorizontalSlider(_fuelTypes[i], -1, _maxFuelType));

                GUILayout.Space(10);

                // Fluid amount.
                GUILayout.Label($"Fuel amount: {_fuelValues[i]}");
                _fuelValues[i] = GUILayout.HorizontalSlider(_fuelValues[i], -1f, 1000f);

                bool fuelValueParse = float.TryParse(GUILayout.TextField(_fuelValues[i].ToString()), out float tempFuelValue);
                if (fuelValueParse)
                    _fuelValues[i] = tempFuelValue;

                GUILayout.EndVertical();
                GUILayout.Space(5);
            }
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if (_fuelMixes <= _maxFuelType && GUILayout.Button("Add fluid"))
            {
                _fuelMixes++;
                _fuelTypes.Add(0);
                _fuelValues.Add(0);
            }
            GUILayout.Space(10);

            if (_fuelMixes > 1 && GUILayout.Button("Remove last fluid"))
            {
                _fuelMixes--;
                _fuelTypes.RemoveAt(_fuelTypes.Count - 1);
                _fuelValues.RemoveAt(_fuelValues.Count - 1);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            Colour.RenderColourSliders(dimensions.width);
            GUILayout.Space(10);

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
