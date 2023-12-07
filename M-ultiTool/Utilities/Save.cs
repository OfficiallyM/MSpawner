﻿using MultiTool.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = MultiTool.Modules.Logger;

namespace MultiTool.Utilities
{
	/// <summary>
	/// Save data utilities.
	/// </summary>
	public static class SaveUtilities
	{
		/// <summary>
		/// Read/write data to game save
		/// <para>Originally from RundensWheelPositionEditor</para>
		/// </summary>
		/// <param name="input">The string to write to the save</param>
		/// <returns>The read/written string</returns>
		public static string ReadWriteToGameSave(string input = null)
		{
			try
			{
				save_rendszam saveRendszam = null;
				save_prefab savePrefab1;

				// Attempt to find existing plate.
				if ((savedatascript.d.data.farStuff.TryGetValue(Mathf.Abs(Meta.ID.GetHashCode()), out savePrefab1) || savedatascript.d.data.nearStuff.TryGetValue(Mathf.Abs(Meta.ID.GetHashCode()), out savePrefab1)) && savePrefab1.rendszam != null)
					saveRendszam = savePrefab1.rendszam;

				// Plate doesn't exist.
				if (saveRendszam == null)
				{
					// Create a new plate to store the input string in.
					tosaveitemscript component = itemdatabase.d.gplate.GetComponent<tosaveitemscript>();
					save_prefab savePrefab2 = new save_prefab(component.category, component.id, double.MaxValue, double.MaxValue, double.MaxValue, 0.0f, 0.0f, 0.0f);
					savePrefab2.rendszam = new save_rendszam();
					saveRendszam = savePrefab2.rendszam;
					saveRendszam.S = string.Empty;
					savedatascript.d.data.farStuff.Add(Mathf.Abs(Meta.ID.GetHashCode()), savePrefab2);
				}

				// Write the input to the plate.
				if (input != null && input != string.Empty)
					saveRendszam.S = input;

				return saveRendszam.S;
			}
			catch (Exception ex)
			{
				Logger.Log($"Save read/write error - {ex}", Logger.LogLevel.Error);
			}

			return string.Empty;
		}

		/// <summary>
		/// Unserialize existing save data
		/// </summary>
		/// <returns>Unserialized save data</returns>
		internal static Save UnserializeSaveData()
		{
			string existingString = ReadWriteToGameSave();
			if (existingString == null || existingString == string.Empty)
				return new Save();

			MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(existingString));
			DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Save));
			return jsonSerializer.ReadObject(ms) as Save;
		}

		/// <summary>
		/// Serialize save data and write to save
		/// </summary>
		/// <param name="data">The data to serialize</param>
		internal static void SerializeSaveData(Save data)
		{
			MemoryStream ms = new MemoryStream();
			DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Save));
			jsonSerializer.WriteObject(ms, data);

			// Rewind stream.
			ms.Seek(0, SeekOrigin.Begin);

			// Convert stream to a string.
			StreamReader reader = new StreamReader(ms);
			string jsonString = reader.ReadToEnd();

			ReadWriteToGameSave(jsonString);
		}

		/// <summary>
		/// Update POI data in save
		/// </summary>
		/// <param name="poi">The POI to update</param>
		/// <param name="type">Update type, either "insert" or "delete"</param>
		/// <returns>POI ID</returns>
		internal static int UpdatePOISaveData(POIData poi, string type = "insert")
		{
			Save data = UnserializeSaveData();

			int ID = -1;

			try
			{
				switch (type)
				{
					case "insert":
						if (data.pois == null)
							data.pois = new List<POIData>();

						poi.ID = data.pois.Count;
						ID = poi.ID;

						data.pois.Add(poi);
						break;
					case "delete":
						POIData poiData = data.pois.First(p => p.ID == poi.ID);
						if (poiData != null)
						{
							ID = poiData.ID;
							data.pois.Remove(poiData);
						}
						break;
				}
			}
			catch (Exception ex)
			{
				Logger.Log($"POI update error - {ex}", Logger.LogLevel.Error);
			}

			SerializeSaveData(data);

			return ID;
		}

		/// <summary>
		/// Update glass data in save
		/// </summary>
		/// <param name="glass">Glass data</param>
		internal static void UpdateGlass(GlassData glass)
		{
			Save data = UnserializeSaveData();

			try
			{
				if (data.glass == null)
					data.glass = new List<GlassData>();

				GlassData existing = data.glass.Where(g => g.ID == glass.ID && g.type == glass.type).FirstOrDefault();
				if (existing != null)
					existing.color = glass.color;
				else
					data.glass.Add(glass);
			}
			catch (Exception ex)
			{
				Logger.Log($"Glass update error - {ex}", Logger.LogLevel.Error);
			}

			SerializeSaveData(data);
		}
	}
}
