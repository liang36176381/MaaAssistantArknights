// <copyright file="ReclamationSettingsUserControlModel.cs" company="MaaAssistantArknights">
// MaaWpfGui - A part of the MaaCoreArknights project
// Copyright (C) 2021 MistEO and Contributors
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License v3.0 only as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY
// </copyright>
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MaaWpfGui.Constants;
using MaaWpfGui.Helper;
using MaaWpfGui.Services;
using MaaWpfGui.Utilities.ValueType;
using MaaWpfGui.ViewModels.UI;
using Newtonsoft.Json.Linq;

namespace MaaWpfGui.ViewModels.UserControl.TaskQueue;

public class ReclamationSettingsUserControlModel : TaskViewModel
{
    static ReclamationSettingsUserControlModel()
    {
        Instance = new();
    }

    public static ReclamationSettingsUserControlModel Instance { get; }

    /// <summary>
    /// Gets the list of reclamation themes.
    /// </summary>
    public List<CombinedData> ReclamationThemeList { get; } =
        [
            new() { Display = $"{LocalizationHelper.GetString("ReclamationThemeFire")} ({LocalizationHelper.GetString("ClosedStage")})", Value = "Fire" },
            new() { Display = LocalizationHelper.GetString("ReclamationThemeTales"), Value = "Tales" },
        ];

    private string _reclamationTheme = ConfigurationHelper.GetValue(ConfigurationKeys.ReclamationTheme, "Tales");

    /// <summary>
    /// Gets or sets the Reclamation theme.
    /// </summary>
    public string ReclamationTheme
    {
        get => _reclamationTheme;
        set
        {
            SetAndNotify(ref _reclamationTheme, value);
            ConfigurationHelper.SetValue(ConfigurationKeys.ReclamationTheme, value);
        }
    }

    /// <summary>
    /// Gets the list of reclamation modes.
    /// </summary>
    public List<CombinedData> ReclamationModeList { get; } =
        [
            new() { Display = LocalizationHelper.GetString("ReclamationModeProsperityNoSave"), Value = "0" },
            new() { Display = LocalizationHelper.GetString("ReclamationModeProsperityInSave"), Value = "1" },
        ];

    private string _reclamationMode = ConfigurationHelper.GetValue(ConfigurationKeys.ReclamationMode, "1");

    /// <summary>
    /// Gets or sets 策略，无存档刷生息点数 / 有存档刷生息点数
    /// </summary>
    public string ReclamationMode
    {
        get => _reclamationMode;
        set
        {
            SetAndNotify(ref _reclamationMode, value);
            ConfigurationHelper.SetValue(ConfigurationKeys.ReclamationMode, value);
        }
    }

    private string _reclamationToolToCraft = ConfigurationHelper.GetValue(ConfigurationKeys.ReclamationToolToCraft, string.Empty).Replace('；', ';');

    public string ReclamationToolToCraft
    {
        get
        {
            if (string.IsNullOrEmpty(_reclamationToolToCraft))
            {
                return LocalizationHelper.GetString("ReclamationToolToCraftPlaceholder", DataHelper.ClientLanguageMapper[SettingsViewModel.GameSettings.ClientType]);
            }

            return _reclamationToolToCraft;
        }

        set
        {
            value = value.Replace('；', ';');
            SetAndNotify(ref _reclamationToolToCraft, value);
            ConfigurationHelper.SetValue(ConfigurationKeys.ReclamationToolToCraft, value);
        }
    }

    private int _reclamationIncrementMode = Convert.ToInt32(ConfigurationHelper.GetValue(ConfigurationKeys.ReclamationIncrementMode, "0"));

    public int ReclamationIncrementMode
    {
        get => _reclamationIncrementMode;
        set
        {
            SetAndNotify(ref _reclamationIncrementMode, value);
            ConfigurationHelper.SetValue(ConfigurationKeys.ReclamationIncrementMode, value.ToString());
        }
    }

    /// <summary>
    /// Gets the list of reclamation increment modes.
    /// </summary>
    public List<CombinedData> ReclamationIncrementModeList { get; } =
        [
            new() { Display = LocalizationHelper.GetString("ReclamationIncrementModeClick"), Value = "0" },
            new() { Display = LocalizationHelper.GetString("ReclamationIncrementModeHold"), Value = "1" },
        ];

    private string _reclamationMaxCraftCountPerRound = ConfigurationHelper.GetValue(ConfigurationKeys.ReclamationMaxCraftCountPerRound, "16");

    public int ReclamationMaxCraftCountPerRound
    {
        get => int.Parse(_reclamationMaxCraftCountPerRound);
        set
        {
            SetAndNotify(ref _reclamationMaxCraftCountPerRound, value.ToString());
            ConfigurationHelper.SetValue(ConfigurationKeys.ReclamationMaxCraftCountPerRound, value.ToString());
        }
    }

    /// <summary>
    /// 自动生息演算。
    /// </summary>
    /// <param name="toolToCraft">要组装的支援道具。</param>
    /// <param name="theme">生息演算主题["Tales"]</param>
    /// <param name="mode">
    /// 策略。可用值包括：
    /// <list type="bullet">
    ///     <item>
    ///         <term><c>0</c></term>
    ///         <description>无存档时通过进出关卡刷生息点数</description>
    ///     </item>
    ///     <item>
    ///         <term><c>1</c></term>
    ///         <description>有存档时通过合成支援道具刷生息点数</description>
    ///     </item>
    /// </list>
    /// </param>
    /// <param name="incrementMode">点击类型：0 连点；1 长按</param>
    /// <param name="numCraftBatches">单次最大制造轮数</param>
    /// <returns>返回(Asst任务类型, 参数)</returns>
    public (AsstTaskType Type, JObject Params) Serialize()
    {
        var data = new JObject
        {
            ["theme"] = ReclamationTheme,
            ["mode"] = ReclamationMode,
            ["increment_mode"] = ReclamationIncrementMode,
            ["num_craft_batches"] = ReclamationMaxCraftCountPerRound,
            ["tools_to_craft"] = new JArray(ReclamationToolToCraft.Split(';').Select(s => s.Trim())),
        };
        return (AsstTaskType.Reclamation, data);
    }
}
