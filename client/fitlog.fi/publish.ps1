Invoke-Command {quasar build -m cordova -T android; cd src-cordova; cordova build android --release -- --keystore="fitlogfi-release.jks" --storePassword=<password> --password=<password>--alias=fitlogfi; cd.. }
md apk -Force
Copy-Item src-cordova\platforms\android\app\build\outputs\apk\release\app-release.apk apk\app-release.apk -Force