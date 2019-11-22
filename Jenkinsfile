pipeline {
  agent {
    docker {
      image 'nosinovacao/dotnet-sonar:latest'
      args '''-v /etc/passwd:/etc/passwd
-v /home/root/.ssh:/root/.ssh'''
    }

  }
  stages {
    stage('Clean and Build') {
      steps {
        sh '''dotnet restore
dotnet /sonar-scanner/SonarScanner.MSBuild.dll begin /k:BA282229-FAC5-4740-B88B-DDBA89359F89 /d:sonar.host.url=https://sonarqube.vandenbrinksoftware.com /d:sonar.login=7fcfcf6e197cb915aa463035592b2de52451bf9a /d:sonar.cs.opencover.reportsPaths=\'tests/**/coverage.opencover.xml\'
dotnet build
dotnet /sonar-scanner/SonarScanner.MSBuild.dll end /d:sonar.login=7fcfcf6e197cb915aa463035592b2de52451bf9a'''
      }
    }

    stage('Publish') {
      parallel {
        stage('Archive') {
          steps {
            archiveArtifacts 'PokerPlanner.API/bin/Release/**'
          }
        }

      }
    }

  }
  environment {
    HOME = '/tmp'
  }
}