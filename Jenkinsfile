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
dotnet publish PokerPlanner.API/PokerPlanner.API.csproj -c Release'''
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