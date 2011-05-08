[T4Scaffolding.Scaffolder(Description = "Makes an EF DbContext able to persist models of a given type, creating the DbContext first if necessary")][CmdletBinding()]
param(        
	[parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string]$ModelType,
	[parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string]$DbContextType,
	[string]$Area,
    [string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders
)

# Ensure we can find the model type
$foundModelType = Get-ProjectType $ModelType -Project $Project
if (!$foundModelType) { return }

# Find the DbContext class, or create it via a template if not already present
$foundDbContextType = Get-ProjectType $DbContextType -Project $Project -AllowMultiple
if (!$foundDbContextType) {
	# Determine where the DbContext class will go
	$defaultNamespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value
	if ($DbContextType.Contains(".")) {
		if ($DbContextType.StartsWith($defaultNamespace + ".", [System.StringComparison]::OrdinalIgnoreCase)) {
			$DbContextType = $DbContextType.Substring($defaultNamespace.Length + 1)
		}
		$outputPath = $DbContextType.Replace(".", [System.IO.Path]::DirectorySeparatorChar)
		$DbContextType = [System.IO.Path]::GetFileName($outputPath)
	} else {
		$outputPath = Join-Path Models $DbContextType
		if ($Area) {
			$areaFolder = Join-Path Areas $Area
			if (-not (Get-ProjectItem $areaFolder -Project $Project)) {
				Write-Error "Cannot find area '$Area'. Make sure it exists already."
				return
			}
			$outputPath = Join-Path $areaFolder $outputPath
		}
	}
	
	$dbContextNamespace = [T4Scaffolding.Namespaces]::Normalize($defaultNamespace + "." + [System.IO.Path]::GetDirectoryName($outputPath).Replace([System.IO.Path]::DirectorySeparatorChar, "."))
	Add-ProjectItemViaTemplate $outputPath -Template DbContext -Model @{
		DefaultNamespace = $defaultNamespace; 
		DbContextNamespace = $dbContextNamespace; 
		DbContextType = $DbContextType; 
	} -SuccessMessage "Added database context '{0}'" -TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

	$foundDbContextType = Get-ProjectType ($dbContextNamespace + "." + $DbContextType) -Project $Project
	if (!$foundDbContextType) { throw "Created database context $DbContextType, but could not find it as a project item" }
} elseif (($foundDbContextType | Measure-Object).Count -gt 1) {
	throw "Cannot find the database context class, because more than one type is called $DbContextType. Try specifying the fully-qualified type name, including namespace."
}

# Add a new property on the DbContext class
if ($foundDbContextType) {
	$propertyName = Get-PluralizedWord $foundModelType.Name
	Add-ClassMemberViaTemplate -Name $propertyName -CodeClass $foundDbContextType -Template DbContextEntityMember -Model @{
		EntityType = $foundModelType;
		EntityTypeNamePluralized = $propertyName;
	} -SuccessMessage "Added '$propertyName' to database context '$($foundDbContextType.FullName)'" -TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage
}

return @{
	DbContextType = $foundDbContextType
}