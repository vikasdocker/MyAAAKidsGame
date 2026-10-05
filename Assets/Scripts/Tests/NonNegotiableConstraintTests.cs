using System;
using System.IO;
using System.Text.RegularExpressions;
using Dab.Runtime.Creature;
using NUnit.Framework;
using UnityEngine;

namespace Dab.Tests.Safety
{
    public sealed class NonNegotiableConstraintTests
    {
        private static readonly string[] ForbiddenRuntimeTypeTokens =
        {
            "GameOver",
            "Defeat",
            "FailState",
            "FailureState",
            "LoseState",
            "EnergySystem",
            "Currency",
            "Streak",
            "LootBox",
            "Purchase",
            "Monetization",
            "Trading",
            "TradeService",
            "Chat",
            "Gift",
            "Social",
            "Leaderboard",
            "Analytics",
            "Advertising",
            "AdService",
            "GameplayNetwork"
        };

        private static readonly Regex GameplayRandomCall = new Regex(
            @"(?:UnityEngine\.)?Random\s*\.|new\s+(?:System\.)?Random\s*\(",
            RegexOptions.Compiled);

        private static readonly Regex CSharpComments = new Regex(
            @"/\*.*?\*/|//[^\r\n]*",
            RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex PackageName = new Regex(
            "\"(?<name>[^\"\\r\\n]+)\"\\s*:",
            RegexOptions.Compiled);

        private static readonly Regex ExternalAdOrAnalyticsSdk = new Regex(
            @"(?i)(?:^|[./:-])(?:admob|applovin|ironsource|unityads|unity\.ads|appsflyer|adjust|amplitude|mixpanel|kochava|singular|analytics|advertising|ads|purchasing|billing)(?:$|[./:-])|firebase[./:-].*(?:analytics|ads)|(?:analytics|ads)[./:-].*firebase",
            RegexOptions.Compiled);

        [Test]
        public void CreatureStateMachineHasNoFailOrLossState()
        {
            var stateNames = Enum.GetNames(typeof(CreatureStateId));
            var forbiddenNames = new[]
            {
                "Fail",
                "Failure",
                "GameOver",
                "Defeat",
                "Lose",
                "Timeout"
            };

            for (var i = 0; i < stateNames.Length; i++)
            {
                for (var j = 0; j < forbiddenNames.Length; j++)
                {
                    Assert.That(
                        stateNames[i].IndexOf(
                            forbiddenNames[j],
                            StringComparison.OrdinalIgnoreCase),
                        Is.EqualTo(-1),
                        $"Creature state '{stateNames[i]}' must not introduce " +
                        $"the forbidden '{forbiddenNames[j]}' outcome.");
                }
            }
        }

        [Test]
        public void RuntimeAssemblyDefinesNoForbiddenMetaSystemTypes()
        {
            var runtimeAssembly = typeof(CreatureStateMachine).Assembly;
            var runtimeTypes = runtimeAssembly.GetTypes();

            for (var i = 0; i < runtimeTypes.Length; i++)
            {
                var type = runtimeTypes[i];
                if (type.Namespace == null ||
                    !type.Namespace.StartsWith("Dab.Runtime", StringComparison.Ordinal))
                {
                    continue;
                }

                for (var j = 0; j < ForbiddenRuntimeTypeTokens.Length; j++)
                {
                    Assert.That(
                        type.Name.IndexOf(
                            ForbiddenRuntimeTypeTokens[j],
                            StringComparison.OrdinalIgnoreCase),
                        Is.EqualTo(-1),
                        $"Runtime type '{type.FullName}' introduces the forbidden " +
                        $"'{ForbiddenRuntimeTypeTokens[j]}' system.");
                }
            }
        }

        [Test]
        public void GameplayDecisionCodeContainsNoRandomRollsAndManifestNoAdOrAnalyticsSdk()
        {
            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            AssertNoRandomCalls(
                Path.Combine(projectRoot, "Assets", "Scripts", "Runtime", "Abilities"));
            AssertNoRandomCalls(
                Path.Combine(projectRoot, "Assets", "Scripts", "Runtime", "Minigames"));
            AssertNoExternalSdkPackages(
                Path.Combine(projectRoot, "Packages", "manifest.json"));
        }

        private static void AssertNoRandomCalls(string directory)
        {
            var sourceFiles = Directory.GetFiles(
                directory,
                "*.cs",
                SearchOption.AllDirectories);

            for (var i = 0; i < sourceFiles.Length; i++)
            {
                var source = File.ReadAllText(sourceFiles[i]);
                source = CSharpComments.Replace(source, string.Empty);
                Assert.That(
                    GameplayRandomCall.IsMatch(source),
                    Is.False,
                    $"Gameplay decision code must not use random rolls: {sourceFiles[i]}");
            }
        }

        private static void AssertNoExternalSdkPackages(string manifestPath)
        {
            var manifest = File.ReadAllText(manifestPath);
            var packageNames = PackageName.Matches(manifest);

            for (var i = 0; i < packageNames.Count; i++)
            {
                var packageName = packageNames[i].Groups["name"].Value;
                if (string.Equals(
                    packageName,
                    "com.unity.modules.unityanalytics",
                    StringComparison.Ordinal))
                {
                    continue;
                }

                Assert.That(
                    ExternalAdOrAnalyticsSdk.IsMatch(packageName),
                    Is.False,
                    $"Third-party ad/analytics SDK '{packageName}' is not allowed.");
            }
        }
    }
}
