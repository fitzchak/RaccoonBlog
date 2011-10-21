param($rootPath, $toolsPath, $package, $project)

if ([T4Scaffolding.ScaffolderAttribute].Assembly.GetName().Version -ne "0.9.9.0") {
	# Abort installation, because we can't update the T4Scaffolding assembly while it's already loaded
	Write-Warning "---"
	Write-Warning "Warning: A different version of T4Scaffolding is already running in this instance of Visual Studio, so installation cannot be completed at this time."
	Write-Warning "You must restart Visual Studio and then install the desired package again. Now rolling back package installation."
	Write-Warning "---"
	$dependents = Get-Package | ?{ ($_.Dependencies | ?{ $_.Id -eq "T4Scaffolding" }) }
	if (!$dependents) { $dependents = @(@{ Id = "MvcScaffolding" }) } # NuGet 1.0 can't tell us the dependencies, so hard-code this important one in that case
	$dependents | %{ try { Uninstall-Package $_.Id -Force } catch {} }
	Uninstall-Package T4Scaffolding -Force
	throw "Installation cannot proceed until you have restarted Visual Studio."
}

# Bail out if scaffolding is disabled (probably because you're running an incompatible version of NuGet)
if (-not (Get-Command Invoke-Scaffolder)) { return }

if ($project) { $projectName = $project.Name }
Get-ProjectItem "InstallationDummyFile.txt" -Project $projectName | %{ $_.Delete() }

Set-DefaultScaffolder -Name DbContext -Scaffolder T4Scaffolding.EFDbContext -SolutionWide -DoNotOverwriteExistingSetting
Set-DefaultScaffolder -Name Repository -Scaffolder T4Scaffolding.EFRepository -SolutionWide -DoNotOverwriteExistingSetting
Set-DefaultScaffolder -Name CustomTemplate -Scaffolder T4Scaffolding.CustomTemplate -SolutionWide -DoNotOverwriteExistingSetting
Set-DefaultScaffolder -Name CustomScaffolder -Scaffolder T4Scaffolding.CustomScaffolder -SolutionWide -DoNotOverwriteExistingSetting
