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
        sh 'dotnet restore'
        sh 'dotnet /sonar-scanner/SonarScanner.MSBuild.dll begin /k:BA282229-FAC5-4740-B88B-DDBA89359F89 /d:sonar.host.url=https://sonarqube.vandenbrinksoftware.com /d:sonar.login=7fcfcf6e197cb915aa463035592b2de52451bf9a /d:sonar.cs.opencover.reportsPaths=\'**/coverage.opencover.xml\' /d:sonar.coverage.exclusions=\'***API/Program.cs,***API/Startup.cs\''
        sh 'dotnet build -c Release'
        sh "rm -drf ${env.WORKSPACE}/testResults"
        sh (returnStatus: true, script: "dotnet test --no-build -c Release --logger trx --results-directory ${env.WORKSPACE}/testResults /p:CollectCoverage=true /p:CoverletOutputFormat=opencover")
        step([$class: 'XUnitPublisher', testTimeMargin: '3000', thresholdMode: 1, thresholds: [[$class: 'FailedThreshold', unstableThreshold: '0']
                            , [$class: 'SkippedThreshold']], tools: [[$class: 'MSTestJunitHudsonTestType', deleteOutputFiles: true, failIfNotNew: false
                            , pattern: 'testResults/**/*.trx', skipNoTestFiles: true, stopProcessingIfError: true]]])
        sh 'dotnet /sonar-scanner/SonarScanner.MSBuild.dll end /d:sonar.login=7fcfcf6e197cb915aa463035592b2de52451bf9a'
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
    DOTNET_CLI_TELEMETRY_OPTOUT = 1
  }
}