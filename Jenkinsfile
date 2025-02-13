pipeline {
    agent { label 'dotnet8' }
    options {
        timeout(time: 1, unit: 'HOURS') 
    }
    triggers {
        pollSCM('* * * * *')
    }
    stages {
        stage('SCM') {
            steps {
                // Correct syntax for checking out the Git repository with a specific branch
                git branch: 'develop', url: 'https://github.com/spandana-26/nopCommerce.git'
            }
        }
        stage('Build') {
            steps {
                sh 'dotnet build -c Release src/Presentation/Nop.Web/Nop.Web.csproj'
                sh 'mkdir published && dotnet publish -o ./published -c Release src/Presentation/Nop.Web/Nop.Web.csproj'
            }
            post {
                success {
                    zip zipFile: './published.zip',
                        archive: true,
                        dir: './published'
                }
            }
        }
    }
}