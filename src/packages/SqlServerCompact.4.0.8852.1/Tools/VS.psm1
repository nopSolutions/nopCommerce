function Get-VsFileSystem {
    $componentModel = Get-VSComponentModel
    $fileSystemProvider = $componentModel.GetService([NuGet.VisualStudio.IFileSystemProvider])
    $solutionManager = $componentModel.GetService([NuGet.VisualStudio.ISolutionManager])
    
    $fileSystem = $fileSystemProvider.GetFileSystem($solutionManager.SolutionDirectory)
    
    return $fileSystem
}

function Add-PostBuildEvent ($project, $installPath) {
    $currentPostBuildCmd = $project.Properties.Item("PostBuildEvent").Value
    $sqlCEPostBuildCmd = Get-PostBuildCommand $installPath
    # Append our post build command if it's not already there
    if (!$currentPostBuildCmd.Contains($sqlCEPostBuildCmd)) {
        $project.Properties.Item("PostBuildEvent").Value += $SqlCEPostBuildCmd
    }
}

function Add-FilesToDirectory ($srcDirectory, $destDirectory) {
    ls $srcDirectory -Recurse -Filter *.dll  | %{
        $srcPath = $_.FullName

        $relativePath = $srcPath.Substring($srcDirectory.Length + 1)
        $destPath = Join-Path $destDirectory $relativePath
        
        $fileSystem = Get-VsFileSystem
        if (!(Test-Path $destPath)) {
            $fileStream = $null
            try {
                $fileStream = [System.IO.File]::OpenRead($_.FullName)
                $fileSystem.AddFile($destPath, $fileStream)
            } catch {
                # We don't want an exception to surface if we can't add the file for some reason
            } finally {
                if ($fileStream -ne $null) {
                    $fileStream.Dispose()
                }
            }
        }
    }
}

function Remove-FilesFromDirectory ($srcDirectory, $destDirectory) {
    $fileSystem = Get-VsFileSystem
    
    ls $srcDirectory -Recurse -Filter *.dll | %{
        $relativePath = $_.FullName.Substring($srcDirectory.Length + 1)
        $fileInBin = Join-Path $destDirectory $relativePath
        if ($fileSystem.FileExists($fileInBin) -and ((Get-Item $fileInBin).Length -eq $_.Length)) {
            # If a corresponding file exists in bin and has the exact file size as the one inside the package, it's most likely the same file.
            try {
                $fileSystem.DeleteFile($fileInBin)
            } catch {
                # We don't want an exception to surface if we can't delete the file
            }
        }
    }
}

function Remove-PostBuildEvent ($project, $installPath) {
    $sqlCEPostBuildCmd = Get-PostBuildCommand $installPath
    
    try {
        # Get the current Post Build Event cmd
        $currentPostBuildCmd = $project.Properties.Item("PostBuildEvent").Value

        # Remove our post build command from it (if it's there)
        $project.Properties.Item("PostBuildEvent").Value = $currentPostBuildCmd.Replace($SqlCEPostBuildCmd, '')
    } catch {
        # Accessing $project.Properties might throw
    }
}

function Get-PostBuildCommand ($installPath) {
    Write-Host $dte.Solution.FullName $installPath
    $solutionDir = [IO.Path]::GetDirectoryName($dte.Solution.FullName) + "\"
    $path = $installPath.Replace($solutionDir, "`$(SolutionDir)")

    $NativeAssembliesDir = Join-Path $path "NativeBinaries"
    $x86 = $(Join-Path $NativeAssembliesDir "x86\*.*")
    $x64 = $(Join-Path $NativeAssembliesDir "amd64\*.*")

    return "
    if not exist `"`$(TargetDir)x86`" md `"`$(TargetDir)x86`"
    xcopy /s /y `"$x86`" `"`$(TargetDir)x86`"
    if not exist `"`$(TargetDir)amd64`" md `"`$(TargetDir)amd64`"
    xcopy /s /y `"$x64`" `"`$(TargetDir)amd64`""
}

function Get-ProjectRoot($project) {
    try {
        $project.Properties.Item("FullPath").Value
    } catch {
        
    }
}

Export-ModuleMember -function Add-PostBuildEvent, Add-FilesToDirectory, Remove-PostBuildEvent, Remove-FilesFromDirectory, Get-ProjectRoot
# SIG # Begin signature block
# MIIbJQYJKoZIhvcNAQcCoIIbFjCCGxICAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUlw9coi3pw/XF39a4+Q8TvDj4
# 16+gghXyMIIEoDCCA4igAwIBAgIKYRr16gAAAAAAajANBgkqhkiG9w0BAQUFADB5
# MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVk
# bW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSMwIQYDVQQDExpN
# aWNyb3NvZnQgQ29kZSBTaWduaW5nIFBDQTAeFw0xMTExMDEyMjM5MTdaFw0xMzAy
# MDEyMjQ5MTdaMIGDMQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQ
# MA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9u
# MQ0wCwYDVQQLEwRNT1BSMR4wHAYDVQQDExVNaWNyb3NvZnQgQ29ycG9yYXRpb24w
# ggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDDqR/PfCN/MR4GJYnddXm5
# z5NLYZK2lfLvqiWdd/NLWm1JkMzgMbimAjeHdK/yrKBglLjHTiX+h9hY0iBOLfE6
# ZS6SW6Zd5pV14DTlUCGcfTmXto5EI2YWpmUg4Dbrivqd4stgAfwqZMiHRRTxHsrN
# KKy65VdZJtzsxUpsmuYDGikyPwCeg6wlDYTM3W+2arst94Q6bWYx6DZw/4SSkPdA
# dp6ILkfWKxH3j+ASZSu8X+8V/PfsAWi3RQzuwASwDre9eGuujeRQ8TXingHS4etb
# cYJhISDz1MneHLgCRWVJvn61N4anzexa37h2IPwRE1H8+ipQqrQe0DqAvmPK3IFH
# AgMBAAGjggEdMIIBGTATBgNVHSUEDDAKBggrBgEFBQcDAzAdBgNVHQ4EFgQUAAOm
# 5aLEcaKCw492zSwNEuKdSigwDgYDVR0PAQH/BAQDAgeAMB8GA1UdIwQYMBaAFFdF
# dBxdsPbIQwXgjFQtjzKn/kiWMFYGA1UdHwRPME0wS6BJoEeGRWh0dHA6Ly9jcmwu
# bWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL01pY0NvZFNpZ1BDQV8wOC0z
# MS0yMDEwLmNybDBaBggrBgEFBQcBAQROMEwwSgYIKwYBBQUHMAKGPmh0dHA6Ly93
# d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljQ29kU2lnUENBXzA4LTMxLTIw
# MTAuY3J0MA0GCSqGSIb3DQEBBQUAA4IBAQCQ9/h5kmnIj2uKYO58wa4+gThS9LrP
# mYzwLT0T9K72YfB1OE5Zxj8HQ/kHfMdT5JFi1qh2FHWUhlmyuhDCf2wVPxkVww4v
# fjnDz/5UJ1iUNWEHeW1RV7AS4epjcooWZuufOSozBDWLg94KXjG8nx3uNUUNXceX
# 3yrgnX86SfvjSEUy3zZtCW52VVWsNMV5XW4C1cyXifOoaH0U6ml7C1V9AozETTC8
# Yvd7peygkvAOKg6vV5spSM22IaXqHe/cCfWrYtYN7DVfa5nUsfB3Uvl36T9smFbA
# XDahTl4Q9Ix6EZcgIDEIeW5yFl8cMFeby3yiVfVwbHjsoUMgruywNYsYMIIEujCC
# A6KgAwIBAgIKYQUTNgAAAAAAGjANBgkqhkiG9w0BAQUFADB3MQswCQYDVQQGEwJV
# UzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UE
# ChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSEwHwYDVQQDExhNaWNyb3NvZnQgVGlt
# ZS1TdGFtcCBQQ0EwHhcNMTEwNzI1MjA0MjE3WhcNMTIxMDI1MjA0MjE3WjCBszEL
# MAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1v
# bmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjENMAsGA1UECxMETU9Q
# UjEnMCUGA1UECxMebkNpcGhlciBEU0UgRVNOOjE1OUMtQTNGNy0yNTcwMSUwIwYD
# VQQDExxNaWNyb3NvZnQgVGltZS1TdGFtcCBTZXJ2aWNlMIIBIjANBgkqhkiG9w0B
# AQEFAAOCAQ8AMIIBCgKCAQEAnDSYGckJKWOZAhZ1qIhXfaG7qUES/GSRpdYFeL93
# 3OzmrrhQTsDjGr3tt/34IIpxOapyknKfignlE++RQe1hJWtRre6oQ7VhQiyd8h2x
# 0vy39Xujc3YTsyuj25RhgFWhD23d2OwW/4V/lp6IfwAujnokumidj8bK9JB5euGb
# 7wZdfvguw2oVnDwUL+fVlMgiG1HLqVWGIbda80ESOZ/wValOqiUrY/uRcjwPfMCW
# ctzBo8EIyt7FybXACl+lnAuqcgpdCkB9LpjQq7KIj4aA6H3RvlVr4FgsyDY/+eYR
# w/BDBYV4AxflLKcpfNPilRcAbNvcrTwZOgLgfWLUzvYdPQIDAQABo4IBCTCCAQUw
# HQYDVR0OBBYEFPaDiyCHEe6Dy9vehaLSaIY3YXSQMB8GA1UdIwQYMBaAFCM0+NlS
# RnAK7UD7dvuzK7DDNbMPMFQGA1UdHwRNMEswSaBHoEWGQ2h0dHA6Ly9jcmwubWlj
# cm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL01pY3Jvc29mdFRpbWVTdGFtcFBD
# QS5jcmwwWAYIKwYBBQUHAQEETDBKMEgGCCsGAQUFBzAChjxodHRwOi8vd3d3Lm1p
# Y3Jvc29mdC5jb20vcGtpL2NlcnRzL01pY3Jvc29mdFRpbWVTdGFtcFBDQS5jcnQw
# EwYDVR0lBAwwCgYIKwYBBQUHAwgwDQYJKoZIhvcNAQEFBQADggEBAGL0BQ1P5xtr
# gudSDN95jKhVgTOX06TKyf6vSNt72m96KE/H0LeJ2NGmmcyRVgA7OOi3Mi/u+c9r
# 2Zje1gL1QlhSa47aQNwWoLPUvyYVy0hCzNP9tPrkRIlmD0IOXvcEnyNIW7SJQcTa
# bPg29D/CHhXfmEwAxLLs3l8BAUOcuELWIsiTmp7JpRhn/EeEHpFdm/J297GOch2A
# djw2EUbKfjpI86/jSfYXM427AGOCnFejVqfDbpCjPpW3/GTRXRjCCwFQY6f889GA
# noTjMjTdV5VAo21+2usuWgi0EAZeMskJ6TKCcRan+savZpiJ+dmetV8QI6N3gPJN
# 1igAclCFvOUwggYHMIID76ADAgECAgphFmg0AAAAAAAcMA0GCSqGSIb3DQEBBQUA
# MF8xEzARBgoJkiaJk/IsZAEZFgNjb20xGTAXBgoJkiaJk/IsZAEZFgltaWNyb3Nv
# ZnQxLTArBgNVBAMTJE1pY3Jvc29mdCBSb290IENlcnRpZmljYXRlIEF1dGhvcml0
# eTAeFw0wNzA0MDMxMjUzMDlaFw0yMTA0MDMxMzAzMDlaMHcxCzAJBgNVBAYTAlVT
# MRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQK
# ExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xITAfBgNVBAMTGE1pY3Jvc29mdCBUaW1l
# LVN0YW1wIFBDQTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAJ+hbLHf
# 20iSKnxrLhnhveLjxZlRI1Ctzt0YTiQP7tGn0UytdDAgEesH1VSVFUmUG0KSrphc
# MCbaAGvoe73siQcP9w4EmPCJzB/LMySHnfL0Zxws/HvniB3q506jocEjU8qN+kXP
# CdBer9CwQgSi+aZsk2fXKNxGU7CG0OUoRi4nrIZPVVIM5AMs+2qQkDBuh/NZMJ36
# ftaXs+ghl3740hPzCLdTbVK0RZCfSABKR2YRJylmqJfk0waBSqL5hKcRRxQJgp+E
# 7VV4/gGaHVAIhQAQMEbtt94jRrvELVSfrx54QTF3zJvfO4OToWECtR0Nsfz3m7IB
# ziJLVP/5BcPCIAsCAwEAAaOCAaswggGnMA8GA1UdEwEB/wQFMAMBAf8wHQYDVR0O
# BBYEFCM0+NlSRnAK7UD7dvuzK7DDNbMPMAsGA1UdDwQEAwIBhjAQBgkrBgEEAYI3
# FQEEAwIBADCBmAYDVR0jBIGQMIGNgBQOrIJgQFYnl+UlE/wq4QpTlVnkpKFjpGEw
# XzETMBEGCgmSJomT8ixkARkWA2NvbTEZMBcGCgmSJomT8ixkARkWCW1pY3Jvc29m
# dDEtMCsGA1UEAxMkTWljcm9zb2Z0IFJvb3QgQ2VydGlmaWNhdGUgQXV0aG9yaXR5
# ghB5rRahSqClrUxzWPQHEy5lMFAGA1UdHwRJMEcwRaBDoEGGP2h0dHA6Ly9jcmwu
# bWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL21pY3Jvc29mdHJvb3RjZXJ0
# LmNybDBUBggrBgEFBQcBAQRIMEYwRAYIKwYBBQUHMAKGOGh0dHA6Ly93d3cubWlj
# cm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljcm9zb2Z0Um9vdENlcnQuY3J0MBMGA1Ud
# JQQMMAoGCCsGAQUFBwMIMA0GCSqGSIb3DQEBBQUAA4ICAQAQl4rDXANENt3ptK13
# 2855UU0BsS50cVttDBOrzr57j7gu1BKijG1iuFcCy04gE1CZ3XpA4le7r1iaHOEd
# AYasu3jyi9DsOwHu4r6PCgXIjUji8FMV3U+rkuTnjWrVgMHmlPIGL4UD6ZEqJCJw
# +/b85HiZLg33B+JwvBhOnY5rCnKVuKE5nGctxVEO6mJcPxaYiyA/4gcaMvnMMUp2
# MT0rcgvI6nA9/4UKE9/CCmGO8Ne4F+tOi3/FNSteo7/rvH0LQnvUU3Ih7jDKu3hl
# XFsBFwoUDtLaFJj1PLlmWLMtL+f5hYbMUVbonXCUbKw5TNT2eb+qGHpiKe+imyk0
# BncaYsk9Hm0fgvALxyy7z0Oz5fnsfbXjpKh0NbhOxXEjEiZ2CzxSjHFaRkMUvLOz
# sE1nyJ9C/4B5IYCeFTBm6EISXhrIniIh0EPpK+m79EjMLNTYMoBMJipIJF9a6lbv
# pt6Znco6b72BJ3QGEe52Ib+bgsEnVLaxaj2JoXZhtG6hE6a/qkfwEm/9ijJssv7f
# UciMI8lmvZ0dhxJkAj0tr1mPuOQh5bWwymO0eFQF1EEuUKyUsKV4q7OglnUa2ZKH
# E3UiLzKoCG6gW4wlv6DvhMoh1useT8ma7kng9wFlb4kLfchpyOZu6qeXzjEp/w7F
# W1zYTRuh2Povnj8uVRZryROj/TCCBoEwggRpoAMCAQICCmEVCCcAAAAAAAwwDQYJ
# KoZIhvcNAQEFBQAwXzETMBEGCgmSJomT8ixkARkWA2NvbTEZMBcGCgmSJomT8ixk
# ARkWCW1pY3Jvc29mdDEtMCsGA1UEAxMkTWljcm9zb2Z0IFJvb3QgQ2VydGlmaWNh
# dGUgQXV0aG9yaXR5MB4XDTA2MDEyNTIzMjIzMloXDTE3MDEyNTIzMzIzMloweTEL
# MAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1v
# bmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEjMCEGA1UEAxMaTWlj
# cm9zb2Z0IENvZGUgU2lnbmluZyBQQ0EwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAw
# ggEKAoIBAQCfjd+FN4yxBlZmNk7UCus2I5Eer6uNWOnEz8GfOgokxMTEXrDuFRTF
# +j6ZM2sZaXL0fAVf5ZklRNc1GYqQ3CiOkAzv1ZBhrd7cGHAtg8lvr4Us+N25uTD9
# cXgcg/3IqbmCZw16uMEJwrwWl1c/HJjTadcwkJCQjTAf2CbUnnuI2eIJ7ZdJResE
# UoF1e7i1IrguVrvXz6lOPAqDoqg6xa22AQ5qzyK0Ix9s1Sfnt37BtNUyrXklHEKG
# 4p2F9FfaG1kvLSaSKcWz14WjnmBalOZ7nHtegjRLbf/U7ifQotzRkAzOfQ4VfIis
# NMfAbJiESslEeWgo3yKDDbiKLEhh4v4RAgMBAAGjggIjMIICHzAQBgkrBgEEAYI3
# FQEEAwIBADAdBgNVHQ4EFgQUV0V0HF2w9shDBeCMVC2PMqf+SJYwCwYDVR0PBAQD
# AgHGMA8GA1UdEwEB/wQFMAMBAf8wgZgGA1UdIwSBkDCBjYAUDqyCYEBWJ5flJRP8
# KuEKU5VZ5KShY6RhMF8xEzARBgoJkiaJk/IsZAEZFgNjb20xGTAXBgoJkiaJk/Is
# ZAEZFgltaWNyb3NvZnQxLTArBgNVBAMTJE1pY3Jvc29mdCBSb290IENlcnRpZmlj
# YXRlIEF1dGhvcml0eYIQea0WoUqgpa1Mc1j0BxMuZTBQBgNVHR8ESTBHMEWgQ6BB
# hj9odHRwOi8vY3JsLm1pY3Jvc29mdC5jb20vcGtpL2NybC9wcm9kdWN0cy9taWNy
# b3NvZnRyb290Y2VydC5jcmwwVAYIKwYBBQUHAQEESDBGMEQGCCsGAQUFBzAChjho
# dHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpL2NlcnRzL01pY3Jvc29mdFJvb3RD
# ZXJ0LmNydDB2BgNVHSAEbzBtMGsGCSsGAQQBgjcVLzBeMFwGCCsGAQUFBwICMFAe
# TgBDAG8AcAB5AHIAaQBnAGgAdAAgAKkAIAAyADAAMAA2ACAATQBpAGMAcgBvAHMA
# bwBmAHQAIABDAG8AcgBwAG8AcgBhAHQAaQBvAG4ALjATBgNVHSUEDDAKBggrBgEF
# BQcDAzANBgkqhkiG9w0BAQUFAAOCAgEAMLywIKRioKfvOSZhPdysxpnQhsQu9YMy
# ZV4iPpvWhvjotp/Ki9Y7dQuhkT5M3WR0jEnyiIwYZ2z+FWZGuDpGQpfIkTfUJLHn
# rNPqQRSDd9PJTwVfoxRSv5akLz5WWxB1zlPDzgVUabRlySSlD+EluBq5TeUCuVAe
# T7OYDB2VAu4iWa0iywV0CwRFewRZ4NgPs+tM+GDdwnie0bqfa/fz7n5EEUDSvbqb
# SxYIbqS+VeSmOBKjSPQcVXqKINF9/pHblI8vwntrpmSFT6PlLDQpXQu/9cc4L8Qg
# xFYx9mnOhfgKkezQ1q66OAUM625PTJwDKaqi/BigKQwNXFxWI1faHJYNyCY2wUTL
# 5eHmb4nnj+mYtXPTeOPtowE8dOVevGz2IYlnBeyXnbWx/a+m6XKlwzThL5/59Go5
# 4i0Eglv80JyufJ0R+ea1Uxl0ujlKOet9QrNKOzc9wkp7J5jn4k6bG0pUOGojN75q
# t0ju6kINSSSRjrcELpdv5OdFu49N/WDZ11nC2IDWYDR7t6GTIP6BuKqlXAnpig2+
# KE1+1+gP7WV40TFfuWbb30LnC8wCB43f/yAGo0VltLMyjS6R4k20qcn6vGsEDrKf
# 6p/epMkKlvSN99iYqPCFAghZpCCmLAsa8lIG7WnlZBgb4KOr3sp8FGFDuGX1NqNV
# EytnLE0bMEwxggSdMIIEmQIBATCBhzB5MQswCQYDVQQGEwJVUzETMBEGA1UECBMK
# V2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0
# IENvcnBvcmF0aW9uMSMwIQYDVQQDExpNaWNyb3NvZnQgQ29kZSBTaWduaW5nIFBD
# QQIKYRr16gAAAAAAajAJBgUrDgMCGgUAoIHKMBkGCSqGSIb3DQEJAzEMBgorBgEE
# AYI3AgEEMBwGCisGAQQBgjcCAQsxDjAMBgorBgEEAYI3AgEVMCMGCSqGSIb3DQEJ
# BDEWBBRaKOb2uBupXmIXOpmViD6BT+efTDBqBgorBgEEAYI3AgEMMVwwWqA4gDYA
# TQBpAGMAcgBvAHMAbwBmAHQAIABBAFMAUAAuAE4ARQBUACAAVwBlAGIAIABQAGEA
# ZwBlAHOhHoAcaHR0cDovL3d3dy5hc3AubmV0L3dlYm1hdHJpeDANBgkqhkiG9w0B
# AQEFAASCAQCybVcAYzQp+COUYT/4R/OExmStHiu9LOXgdYzadez6/LErLrfE2vzt
# VLNNOglDjGp0PKW94UMZwzV6yff1uykLNkfCJ3jJZIMcgj5w8L5LJn7t/0pwb80w
# Lj7Dabej9N3NPheWtf8kEzaJEFwZTck2GJyfDTXN1M1JhcAzsJdAODsrP3zh3h+F
# 9J8CE5y6VJxMZB593chqan8j+ry/akQXnk4SSpir603hY3xL6BMNvccbnEcwuGZU
# aNAigkQxkG50GdLHez44CXLBxsvbMPP2FVmZaHvwsm0U+r5lRQ+tneRYUaUzGBh5
# kjHtOsPJOEUZFwvwf6xoLqsJ2WnrJ3W3oYICHTCCAhkGCSqGSIb3DQEJBjGCAgow
# ggIGAgEBMIGFMHcxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAw
# DgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24x
# ITAfBgNVBAMTGE1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQQIKYQUTNgAAAAAAGjAH
# BgUrDgMCGqBdMBgGCSqGSIb3DQEJAzELBgkqhkiG9w0BBwEwHAYJKoZIhvcNAQkF
# MQ8XDTEyMDEwNTE4NTUwNVowIwYJKoZIhvcNAQkEMRYEFDH/7pQUSuv2BYNBGgSc
# BNUCUMtbMA0GCSqGSIb3DQEBBQUABIIBAFUHf3sHwFuDuPvsZacdY2TXtPikHXK5
# 5Kfaa9P/xChSK7GUs3o+Fgqymu+lWIGAjBIdccuWtzQO1U6myDw2MV/rH4Fo7nr5
# i5Krj0WJ5lhB3nye1RRsem2SXF6bwVau5C30CFgA3Jiltv19itH/thaBMuZse5SY
# 3XmgsFLH0CKA2r8iB4SAVdRKuPDtstAlIIo3FMQoNf540SV6Ct3CMsSXgGK3JX9a
# wqHoY+OClLmzL0GVvNKTD40aTQEnn6DJLtzZqKIK3F9xLOrIXMapHWv0mKdgyy3z
# p2cZ+1sqBaBnoeltnGl3pWTsaGQqaPNjM6SOmTxyUOE7OQMOOMC6ENc=
# SIG # End signature block
