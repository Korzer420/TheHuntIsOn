using GlobalEnums;
using Hkmp.Api.Client;
using Modding;
using Modding.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheHuntIsOn.Modules.PauseTimerModule;
using TMPro;
using UnityEngine;

namespace TheHuntIsOn.Modules.PauseModule;

internal class CountdownsDisplayer
{
    private GameObject parent;
    private List<TextMeshPro> textMeshProCache = [];

    private IClientApi clientApi;
    private float respawnTimer;

    private bool EnsureParent()
    {
        if (parent != null) return true;

        var camera = GameObject.Find("_GameCameras/HudCamera");
        if (camera == null) return false;

        parent = new("CountdownsDisplayer");
        Object.DontDestroyOnLoad(parent);
        parent.transform.SetParent(camera.transform);
        parent.transform.position = Vector3.zero;
        return true;
    }

    internal void SetClientApi(IClientApi clientApi) => this.clientApi = clientApi;

    internal void Enable()
    {
        On.GameManager.Update += OnGMUpdate;
        On.GameManager.BeginSceneTransitionRoutine += OnBeginSceneTransitionRoutine;
        ModHooks.BeforePlayerDeadHook += BeforeHeroDeath;
    }

    internal void Disable()
    {
        if (parent != null)
        {
            Object.Destroy(parent);
            parent = null;
            textMeshProCache.Clear();
        }

        On.GameManager.Update -= OnGMUpdate;
        On.GameManager.BeginSceneTransitionRoutine -= OnBeginSceneTransitionRoutine;
        ModHooks.BeforePlayerDeadHook -= BeforeHeroDeath;
    }

    private static string FormatTime(float timeSeconds)
    {
        if (timeSeconds <= 0) return "0.00";
        if (timeSeconds >= 3600)
        {
            int hours = Mathf.FloorToInt(timeSeconds / 3600);
            int minutes = Mathf.FloorToInt((timeSeconds % 3600) / 60);
            if (minutes >= 60) minutes = 59;

            string status = $"{hours} {(hours > 1 ? "hours" : "hour")}";
            if (minutes > 0) status = $"{status} and {minutes} {(minutes > 1 ? "minutes" : "minute")}";
            return status;
        }
        if (timeSeconds >= 60)
        {
            int minutes = Mathf.FloorToInt(timeSeconds / 60);
            int seconds = Mathf.FloorToInt(timeSeconds % 60);
            if (seconds >= 60) seconds = 59;
            return $"{minutes}:{seconds:00}";
        }
        if (timeSeconds >= 10) return $"{timeSeconds:00.0}";
        return $"{timeSeconds:0.00}";
    }

    private List<string> ComputeStatuses()
    {
        List<string> statuses = [];

        var saveData = TheHuntIsOn.LocalSaveData;
        if (saveData.IsServerPaused(out var unpauseSeconds))
        {
            if (!unpauseSeconds.HasValue) statuses.Add("Server Paused");
            else statuses.Add($"Unpausing in: {FormatTime(unpauseSeconds.Value)}");
        }
        if (respawnTimer > 0) statuses.Add($"Respawn in: {FormatTime(respawnTimer)}");

        foreach (var countdown in saveData.GlobalCountdowns)
        {
            if (countdown.GetDisplayTime(out float seconds)) statuses.Add($"{countdown.Message}: {FormatTime(seconds)}");
        }

        return statuses;
    }

    private static TMP_FontAsset font;
    private static TMP_FontAsset LoadFontAsset() => Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault(font => font.name == "trajan_bold_tmpro");

    private void CreateTextMesh()
    {
        GameObject obj = new("Display");
        obj.transform.SetParent(parent.transform);

        var mesh = obj.AddComponent<TextMeshPro>();
        mesh.font = (font ??= LoadFontAsset());
        mesh.color = Color.white;
        mesh.enableWordWrapping = false;
        mesh.autoSizeTextContainer = true;

        obj.layer = (int)PhysLayers.UI;
        var renderer = obj.GetOrAddComponent<MeshRenderer>();
        renderer.sortingLayerName = "HUD";
        renderer.sortingOrder = 11;

        obj.SetActive(false);
        textMeshProCache.Add(mesh);
    }

    private void UpdateStatuses(List<string> statuses)
    {
        var saveData = TheHuntIsOn.GlobalSaveData;
        var spacingParameters = saveData.PauseTimerPosition.SpacingParameters();

        for (int i = 0; i < textMeshProCache.Count; i++)
        {
            var mesh = textMeshProCache[i];
            var status = i < statuses.Count ? statuses[i] : "";
            if (status == "")
            {
                mesh.gameObject.SetActive(false);
                mesh.text = "";
                mesh.transform.localPosition = new(-100, -100);
                continue;
            }

            mesh.gameObject.SetActive(true);
            mesh.text = status;
            mesh.fontSize = 24;

            var scale = saveData.PauseTimerSize.FontScale();
            mesh.transform.localScale = new(scale, scale, 1);

            mesh.ForceMeshUpdate();
            var bounds = mesh.textBounds;
            mesh.transform.localPosition = spacingParameters.GetPosition(i, statuses.Count, saveData.PauseTimerSize.Spacing(), scale, bounds);
        }
    }

    private bool IsConnected() => clientApi != null && clientApi.NetClient.IsConnected;

    private void OnGMUpdate(On.GameManager.orig_Update orig, GameManager self)
    {
        orig(self);

        if (respawnTimer > 0 && IsConnected() && !TheHuntIsOn.LocalSaveData.IsServerPaused(out _))
        {
            respawnTimer -= Time.unscaledDeltaTime;
            if (respawnTimer < 0) respawnTimer = 0;
        }

        if (!EnsureParent()) return;

        List<string> statuses = [];
        if (IsConnected()) statuses = ComputeStatuses();
        while (textMeshProCache.Count < statuses.Count) CreateTextMesh();
        UpdateStatuses(statuses);
    }

    private IEnumerator OnBeginSceneTransitionRoutine(On.GameManager.orig_BeginSceneTransitionRoutine orig, GameManager self, GameManager.SceneLoadInfo sceneLoadInfo)
    {
        var src = orig(self, sceneLoadInfo);
        if (respawnTimer > 0 && IsConnected())
        {
            IEnumerator Modified()
            {
                yield return new WaitUntil(() => respawnTimer <= 0 || !IsConnected());
                while (src.MoveNext()) yield return src.Current;
            }
            return Modified();
        }
        else return src;
    }

    private void BeforeHeroDeath() => respawnTimer = Mathf.Max(respawnTimer, TheHuntIsOn.LocalSaveData.RespawnTimerSeconds);
}
