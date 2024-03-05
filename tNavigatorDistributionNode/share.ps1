$NewAcl = Get-Acl -Path "C:\Users\KosachevIV\Desktop\test"
$fileSystemAccessRuleArgumentList = "Все", "FullControl", 'ContainerInherit,ObjectInherit', 'None', 'Allow'
$fileSystemAccessRule = New-Object -TypeName System.Security.AccessControl.FileSystemAccessRule -ArgumentList $fileSystemAccessRuleArgumentList
$NewAcl.SetAccessRule($fileSystemAccessRule)
Set-Acl -Path "C:\Users\KosachevIV\Desktop\test" -AclObject $NewAcl

