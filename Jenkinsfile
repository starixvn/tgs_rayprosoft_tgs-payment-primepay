pipeline {
	agent any
	
	environment {
		gitlab_user_pwd=credentials('gitlab-user-pwd')
		BuildConfiguration="Debug"
		ImageName="registry.gitlab.com/rayprosoft/turnkey-gaming-system/leepay"
	}

	stages {
		stage('Build') {
			steps {
				sh 'dotnet build -c $BuildConfiguration'
			}
		}

		stage('Test') {
			steps {
				sh 'dotnet test -c $BuildConfiguration --logger trx --no-build'
			}
		}
		
		stage('Push packages') {
			steps {
				sh 'dotnet pack -c $BuildConfiguration "Tgs.Payment.PrimePay.Client/Tgs.Payment.PrimePay.Client.csproj" --no-build'
				sh 'dotnet nuget push "Tgs.Payment.PrimePay.Client/bin/$BuildConfiguration/*.nupkg" --source "raypro" --skip-duplicate'
			}
		}
	}

	post {
		always {
			step([$class: 'MSTestPublisher', testResultsFile:"**/*.trx", failOnError: false, keepLongStdio: true])
		}
	}
}
