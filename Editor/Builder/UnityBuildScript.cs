using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace NovaEngine.Editor
{
    public static class UnityBuildScript
    {
        // 构建方法，从命令行调用
        public static void Build()
        {
            try
            {
                // 获取命令行参数
                string buildTargetStr = GetArgValue("-buildTarget");
                string buildName = GetArgValue("-buildName");
                string buildVersion = GetArgValue("-buildVersion");
                string buildNumberStr = GetArgValue("-buildNumber");
                string outputDir = GetArgValue("-outputDir");

                // 获取场景列表
                List<string> scenes = GetSceneListFromArgs();

                // 解析构建目标
                BuildTarget buildTarget = ParseBuildTarget(buildTargetStr);

                // 解析构建选项
                BuildOptions options = BuildOptions.None;
                if (GetArgFlag("-developmentBuild"))
                    options |= BuildOptions.Development;
                if (GetArgFlag("-allowDebugging"))
                    options |= BuildOptions.AllowDebugging;
                if (GetArgFlag("-enableProfiler"))
                    options |= BuildOptions.EnableDeepProfilingSupport;
                if (GetArgFlag("-scriptDebugging"))
                    options |= BuildOptions.WaitForPlayerConnection;

                // 设置PlayerSettings
                SetPlayerSettings(buildName, buildVersion, buildNumberStr);

                // 准备构建路径
                string buildPath = PrepareBuildPath(outputDir, buildTarget, buildName, buildVersion, buildNumberStr);

                // 执行构建
                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
                {
                    scenes = scenes.ToArray(),
                    locationPathName = buildPath,
                    target = buildTarget,
                    options = options
                };

                // 平台特定设置
                ApplyPlatformSpecificSettings(buildTarget);

                Debug.Log($"Starting build: {buildTarget} to {buildPath}");

                BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

                if (report.summary.result == BuildResult.Succeeded)
                {
                    Debug.Log($"Build succeeded! Size: {report.summary.totalSize} bytes");

                    // 生成构建信息文件
                    GenerateBuildInfo(buildPath, buildName, buildVersion, buildNumberStr, report);

                    // 通知外部程序构建完成
                    NotifyBuildComplete(true, buildPath, (long) report.summary.totalSize);
                }
                else
                {
                    Debug.LogError($"Build failed!");
                    NotifyBuildComplete(false, "", 0);
                    EditorApplication.Exit(1);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Build error: {e}");
                NotifyBuildComplete(false, "", 0);
                EditorApplication.Exit(1);
            }
        }

        private static string GetArgValue(string argName)
        {
            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == argName && i + 1 < args.Length)
                {
                    return args[i + 1];
                }
            }
            return null;
        }

        private static bool GetArgFlag(string argName)
        {
            return System.Environment.GetCommandLineArgs().Contains(argName);
        }

        private static List<string> GetSceneListFromArgs()
        {
            List<string> scenes = new List<string>();
            string[] args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-scene" && i + 1 < args.Length)
                {
                    scenes.Add(args[i + 1]);
                }
            }

            // 如果没有指定场景，使用Build Settings中的场景
            if (scenes.Count == 0)
            {
                scenes.AddRange(EditorBuildSettings.scenes
                    .Where(s => s.enabled)
                    .Select(s => s.path));
            }

            return scenes;
        }

        private static BuildTarget ParseBuildTarget(string targetStr)
        {
            if (Enum.TryParse(targetStr, out BuildTarget target))
            {
                return target;
            }

            // 默认值
            return BuildTarget.StandaloneWindows64;
        }

        private static void SetPlayerSettings(string productName, string version, string buildNumber)
        {
            if (!string.IsNullOrEmpty(productName))
                PlayerSettings.productName = productName;

            if (!string.IsNullOrEmpty(version))
                PlayerSettings.bundleVersion = version;

            if (int.TryParse(buildNumber, out int buildNum))
                PlayerSettings.iOS.buildNumber = buildNumber;
        }

        private static string PrepareBuildPath(string outputDir, BuildTarget target,
                                              string name, string version, string buildNumber)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string buildName = $"{name}_{version}_{buildNumber}_{timestamp}";

            string extension = GetBuildExtension(target);
            string fileName = $"{name}{extension}";

            string fullPath = Path.Combine(outputDir, target.ToString(), buildName, fileName);

            // 确保目录存在
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            return fullPath;
        }

        private static string GetBuildExtension(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return ".exe";
                case BuildTarget.StandaloneOSX:
                    return ".app";
                case BuildTarget.Android:
                    return ".apk";
                case BuildTarget.iOS:
                    return "";
                case BuildTarget.WebGL:
                    return "";
                case BuildTarget.StandaloneLinux64:
                    return ".x86_64";
                default:
                    return "";
            }
        }

        private static void ApplyPlatformSpecificSettings(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    // Android设置
                    EditorUserBuildSettings.buildAppBundle = false;
                    PlayerSettings.Android.bundleVersionCode =
                        int.Parse(PlayerSettings.iOS.buildNumber);
                    break;

                case BuildTarget.iOS:
                    // iOS设置
                    PlayerSettings.iOS.appleEnableAutomaticSigning = true;
                    break;

                case BuildTarget.WebGL:
                    // WebGL设置
                    PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
                    PlayerSettings.WebGL.decompressionFallback = true;
                    break;
            }
        }

        private static void GenerateBuildInfo(string buildPath, string name,
                                             string version, string buildNumber, BuildReport report)
        {
            string buildDir = Path.GetDirectoryName(buildPath);
            string infoPath = Path.Combine(buildDir, "build_info.json");

            var buildInfo = new
            {
                productName = name,
                version = version,
                buildNumber = buildNumber,
                buildTime = DateTime.Now.ToString("o"),
                buildTarget = EditorUserBuildSettings.activeBuildTarget.ToString(),
                unityVersion = UnityEngine.Application.unityVersion,
                totalSize = report.summary.totalSize,
                outputPath = buildPath
            };

            string json = JsonUtility.ToJson(buildInfo, true);
            File.WriteAllText(infoPath, json);
        }

        private static void NotifyBuildComplete(bool success, string outputPath, long size)
        {
            // 可以通过文件、网络或环境变量通知外部程序
            string resultFile = "build_result.json";
            var result = new
            {
                success = success,
                outputPath = outputPath,
                size = size,
                timestamp = DateTime.Now.ToString("o")
            };

            string json = JsonUtility.ToJson(result, true);
            File.WriteAllText(resultFile, json);
        }
    }
}
