pipeline {
  environment {
    HOME = '/tmp'
    DOTNET_CLI_TELEMETRY_OPTOUT = 1
    MSBUILD_SQ_SCANNER_HOME = tool 'SonarQubeScannerMSBuild'
  }

  agent {
    dockerfile {
      filename 'Dockerfile'
      args '--net jenkins'
    }
  }
  stages {
    stage('Restore NuGet Packages') {
      steps {
        sh 'dotnet restore'
      }
    }
    stage('Begin SonarQube') {
      steps {
        withSonarQubeEnv('sonarqube') {
          sh "dotnet ${MSBUILD_SQ_SCANNER_HOME}/SonarScanner.MSBuild.dll begin /k:pokerplanner-backend /d:sonar.host.url=http://sonarqube:9000 /d:sonar.login=99eb50ab35d96468c912a975155d65a9086d72b1 /d:sonar.cs.opencover.reportsPaths=\'**/coverage.opencover.xml\' /d:sonar.branch.name=${BRANCH_NAME} /d:sonar.coverage.exclusions=\'***API/Program.cs,***API/Startup.cs\'"
        }
      }
    }
    stage('Build Code') {
      steps {
        sh 'dotnet build -c Release'
        sh "rm -drf ${env.WORKSPACE}/testResults"
      }
    }
    stage('Run Unit Tests') {
      steps {
        sh (returnStatus: true, script: "dotnet test --no-build -c Release --logger trx --results-directory ${env.WORKSPACE}/testResults /p:CollectCoverage=true /p:CoverletOutputFormat=opencover")
        step([$class: 'XUnitPublisher', testTimeMargin: '3000', thresholdMode: 1, thresholds: [[$class: 'FailedThreshold', unstableThreshold: '0']
              , [$class: 'SkippedThreshold']], tools: [[$class: 'MSTestJunitHudsonTestType', deleteOutputFiles: true, failIfNotNew: false
              , pattern: 'testResults/**/*.trx', skipNoTestFiles: true, stopProcessingIfError: true]]])
      }
    }
    stage('End SonarQube') {
      steps {
        withSonarQubeEnv('sonarqube') {
          sh "dotnet ${MSBUILD_SQ_SCANNER_HOME}/SonarScanner.MSBuild.dll end /d:sonar.login=99eb50ab35d96468c912a975155d65a9086d72b1"
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