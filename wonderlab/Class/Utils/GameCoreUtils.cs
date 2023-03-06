﻿using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace wonderlab.Class.Utils
{
    public class GameCoreUtils {   
        public static async ValueTask<ObservableCollection<GameCore>> GetLocalGameCores(string root) { 
            var cores = await Task.Run(() => {
                return new GameCoreToolkit(root).GetGameCores();
            });

            return cores is null ? new ObservableCollection<GameCore>() : cores.ToObservableCollection();
        }

        public static async ValueTask<ObservableCollection<GameCore>> SearchGameCoreAsync(string root,string text) { 
            var cores = await Task.Run(() => {
                try {
                    return new GameCoreToolkit(root).GameCoreScearh(text);
                }
                catch {}

                return null;
            });

            return cores is null ? new ObservableCollection<GameCore>() : cores.ToObservableCollection();
        }
    }
}
