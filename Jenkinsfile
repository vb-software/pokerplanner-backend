node('master') {
    stage('Set Env') {
      MSBUILD_SQ_SCANNER_HOME = tool 'SonarQubeScannerMSBuild'
    }
}

pipeline {
  environment {
    HOME = '/tmp'
    DOTNET_CLI_TELEMETRY_OPTOUT = 1
  }

  agent {
    docker {
      image 'mcr.microsoft.com/dotnet/core/sdk:3.0'
      // args "-v ${MSBUILD_SQ_SCANNER_HOME}:/opt/sonarscanner"
    }

  }
  stages {
    stage('Clean and Build') {
      steps {
        withSonarQubeEnv('sonarqube') {
          sh 'dotnet restore'
          sh 'dotnet ${MSBUILD_SQ_SCANNER_HOME}/SonarScanner.MSBuild.dll begin /k:BA282229-FAC5-4740-B88B-DDBA89359F89 /d:sonar.host.url=https://sonarqube.vandenbrinksoftware.com /d:sonar.login=7fcfcf6e197cb915aa463035592b2de52451bf9a /d:sonar.cs.opencover.reportsPaths=\'**/coverage.opencover.xml\' /d:sonar.branch.name=${BRANCH_NAME} /d:sonar.coverage.exclusions=\'***API/Program.cs,***API/Startup.cs\''
          sh 'dotnet build -c Release'
          sh "rm -drf ${env.WORKSPACE}/testResults"
          sh (returnStatus: true, script: "dotnet test --no-build -c Release --logger trx --results-directory ${env.WORKSPACE}/testResults /p:CollectCoverage=true /p:CoverletOutputFormat=opencover")
          step([$class: 'XUnitPublisher', testTimeMargin: '3000', thresholdMode: 1, thresholds: [[$class: 'FailedThreshold', unstableThreshold: '0']
                              , [$class: 'SkippedThreshold']], tools: [[$class: 'MSTestJunitHudsonTestType', deleteOutputFiles: true, failIfNotNew: false
                              , pattern: 'testResults/**/*.trx', skipNoTestFiles: true, stopProcessingIfError: true]]])
          sh 'dotnet ${MSBUILD_SQ_SCANNER_HOME}/SonarScanner.MSBuild.dll end /d:sonar.login=7fcfcf6e197cb915aa463035592b2de52451bf9a'
        }

        timeout(time: 10, unit: 'MINUTES') {
          waitForQualityGate true
        }
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
}