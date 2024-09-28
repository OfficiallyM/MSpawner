﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace MultiTool.Core
{
	[DataContract]
	internal class POIData
	{
		[DataMember] public int ID { get; set; }
		[DataMember] public string poi { get; set; }
		[DataMember] public Vector3 position { get; set; }
		[DataMember] public Quaternion rotation { get; set; }
	}

	[DataContract]
	internal class GlassData
	{
		[DataMember] public uint ID { get; set; }
		[DataMember] public Color color { get; set; }
		[DataMember] public string type { get; set; }
	}

	[DataContract]
	internal class MaterialData
	{
		[DataMember] public uint ID { get; set; }
		[DataMember] public string part { get; set; }
        [DataMember] public bool? isConditionless { get; set; } = false;
		[DataMember] public bool exact { get; set; }
		[DataMember] public string type { get; set; }
		[DataMember] public Color? color { get; set; }
	}

	[DataContract]
	internal class ScaleData
	{
		[DataMember] public uint ID { get; set; }
		[DataMember] public Vector3 scale { get; set; }
	}

	[DataContract]
	internal class SlotData
	{
		[DataMember] public uint ID { get; set; }
		[DataMember] public string slot { get; set; }
		[DataMember] public Vector3 position { get; set; }
		[DataMember] public Vector3 resetPosition { get; set; }
		[DataMember] public Quaternion rotation { get; set; }
		[DataMember] public Quaternion resetRotation { get; set; }
	}

    [DataContract]
    internal class LightData
    {
        [DataMember] public uint ID { get; set; }
        [DataMember] public string name { get; set; }
        [DataMember] public Color color { get; set; }
    }

    [DataContract]
    internal class EngineTuningData
    {
        [DataMember] public uint ID { get; set; }
        [DataMember] public EngineTuning tuning { get; set; }
    }

    [DataContract]
    internal class TransmissionTuningData
    {
        [DataMember] public uint ID { get; set; }
        [DataMember] public TransmissionTuning tuning { get; set; }
    }

    [DataContract]
    internal class VehicleTuningData
    {
        [DataMember] public uint ID { get; set; }
        [DataMember] public VehicleTuning tuning { get; set; }
    }

    [DataContract]
	internal class Save
	{
		[DataMember] public List<POIData> pois { get; set; }
		[DataMember] public List<GlassData> glass { get; set; }
		[DataMember] public List<MaterialData> materials { get; set; }
		[DataMember] public List<ScaleData> scale { get; set; }
		[DataMember] public List<SlotData> slots { get; set; }
        [DataMember] public List<LightData> lights { get; set; }
        [DataMember] public List<EngineTuningData> engineTuning { get; set; }
        [DataMember] public List<TransmissionTuningData> transmissionTuning { get; set; }
        [DataMember] public List<VehicleTuningData> vehicleTuning { get; set; }
    }
}
