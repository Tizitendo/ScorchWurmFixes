using BepInEx;
using Logger;
using RoR2;
using RoR2.CharacterAI;
using RoR2.ContentManagement;
using RoR2BepInExPack.GameAssetPathsBetter;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScorchWurmFixes;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]

public class ScorchWurmFixes : BaseUnityPlugin
{
    public const string PluginGUID = PluginAuthor + "." + PluginName;
    public const string PluginAuthor = "Onyx";
    public const string PluginName = "ScorchWurmFixes";
    public const string PluginVersion = "1.0.0";

	static Vector3 detectionBounds = new(0.5f, 2f, 0.5f);
	static Vector3 yOffset = Vector3.up * (detectionBounds.y + 0.2f);

	public void Awake()
    {
		Log.Init(Logger);

		AssetReferenceT<GameObject> moonMatRef = new(RoR2_DLC2_Scorchling.ScorchlingMaster_prefab);
		AssetAsyncReferenceManager<GameObject>.LoadAsset(moonMatRef).Completed += (x) =>
		{
			foreach (AISkillDriver component in x.Result.GetComponents<AISkillDriver>())
			{
				if (component.customName == "Breach")
				{
					component.enabled = false;
				}
				if (component.customName == "ChaseOffNodegraph")
				{
					component.enabled = false;
				}
				if (component.customName == "ChaseOffNodegraphClose")
				{
					component.maxDistance = 50f;
				}
				if (component.customName == "FollowNodeGraphToTarget")
				{
					component.minDistance = 0;
					component.driverUpdateTimerOverride = 1f;
				}
			}
		};

		On.EntityStates.Scorchling.ScorchlingBreach.OnEnter += (orig, self) =>
        {
			if (Physics.CheckBox(self.characterBody.footPosition + yOffset, detectionBounds, Quaternion.identity, LayerIndex.world.mask))
			{
				self.outer.SetNextStateToMain();
				return;
			}

			orig(self);
		};
	}
}
