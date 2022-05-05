// Copyright (c) SharpCrafters s.r.o. All rights reserved.
// This project is not open source. Please see the LICENSE.md file in the repository root for details.

using PostSharp.Engineering.BuildTools;
using PostSharp.Engineering.BuildTools.Build.Model;
using PostSharp.Engineering.BuildTools.Build.Solutions;
using PostSharp.Engineering.BuildTools.Dependencies.Model;
using Spectre.Console.Cli;

var productDependencyDefinition =
    new DependencyDefinition("Metalama.Tests.NopCommerce", VcsProvider.GitHub, "Metalama", false);

var product = new Product(productDependencyDefinition)
{
    ProductName = productDependencyDefinition.Name,
    Solutions = new Solution[]
    {
        new DotNetSolution("src\\NopCommerce.sln"),
    },
    PublicArtifacts = Pattern.Create("Nop.Web.Framework.4.5.0.nupkg"),
    Dependencies = new[] { Dependencies.PostSharpEngineering, Dependencies.Metalama },
};

var commandApp = new CommandApp();

commandApp.AddProductCommands( product );

return commandApp.Run( args );