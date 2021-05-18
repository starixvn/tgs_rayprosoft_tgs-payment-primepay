pipeline {
	agent any
	
	environment {
		gitlab_user_pwd=credentials('gitlab-user-pwd')
		BuildConfiguration="Debug"
		ImageName="registry.gitlab.com/rayprosoft/turnkey-gaming-system/primepay"
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

		stage('Build Docker Image') {
			steps {
				sh 'dotnet build "Tgs.Payment.PrimePay/Tgs.Payment.PrimePay.csproj" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageName:v${BUILD_NUMBER}" --no-restore '
			}
		}

		stage('Push Docker Image') {
			steps {
				sh 'sudo docker push $ImageName:v${BUILD_NUMBER}'
			}
		}

		stage('Deploy to dev') {
			input {
				message 'Ready to go?'
				ok 'yes'
			}

			steps {
				configFileProvider([
					configFile(
						fileId: 'PrimePay.dev.json', 
						variable: 'primepaydev')]) {
					script {
						def json = readJSON file: "$primepaydev";

						env.TGS = json.tgs;
						env.PaymentApiUrl = json.paymentApiUrl;
						env.NatsUri = json.nats;

						withKubeConfig([credentialsId: 'k8s-kube-config', serverUrl: 'https://192.168.50.105:6443']) {
							sh '''
								helm template k8s/primepay \
									--set image.tag=v${BUILD_NUMBER} \
									--set namespace=dev \
									--set "connectionStrings.tgs=${TGS}" \
									--set nats=${NatsUri}
							'''
							sh '''
								helm template k8s/primepay \
									--set image.tag=v${BUILD_NUMBER} \
									--set namespace=dev \
									--set "connectionStrings.tgs=${TGS}" \
									--set nats=${NatsUri} | kubectl -n dev apply -f -
							'''
						}
					}
				}
			}
		}
	}

	post {
		always {
			step([$class: 'MSTestPublisher', testResultsFile:"**/*.trx", failOnError: false, keepLongStdio: true])
		}
	}
}
