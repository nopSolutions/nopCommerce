// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using PostSharp.Engineering.BuildTools;
using PostSharp.Engineering.BuildTools.Build.Model;
using PostSharp.Engineering.BuildTools.Build.Solutions;
using PostSharp.Engineering.BuildTools.Dependencies.Model;
using Spectre.Console.Cli;

var product = new Product(TestDependencies.NopCommerce)
{
    Solutions = new Solution[]
    {
        new DotNetSolution("src\\NopCommerce.sln") { BuildMethod = BuildMethod.Build },
    },
    Dependencies = new[] { Dependencies.PostSharpEngineering, Dependencies.Metalama },
};

var commandApp = new CommandApp();

commandApp.AddProductCommands( product );

return commandApp.Run( args );