import constants from '../../store/constants'
import utils from '../../utils'
import config from '../../config'
import api from '../../api'
import moment from 'moment'
import PageMixin from '../../mixins/page'

export default {
    mixins: [PageMixin],
    data () {
        return {
            tab: 'tab-1',
            pals: [
                { value: 1.2, name: '1.2 Makaaminen s&auml;ngyssä' }
            ],
            genders: [],
            dob: undefined,
            /*
            day: undefined,
            month: undefined,
            year: undefined,
            */
            gender: undefined,
            height: undefined,
            weight: undefined,
            rmr: undefined,
            rmrSpecified: false,
            pal: undefined,

            username: undefined,
            hasPassword: false,
            hasFacebook: false,
            hasGoogle: false,
            oldPassword: undefined,
            newPassword: undefined,
            newPassword2: undefined,
            usernameError: null,
            passwordError: null,
            password2Error: null,

            deleteStarted: false,
            info: ''
        }
    },
    computed: {
        /*
        dob() {
            if (this.year && this.month && this.day) {
                return new Date(this.year, this.month - 1, this.day)
            }
            return undefined;
        },
        */
        rmrEstimate() {
            if (!this.gender || !this.height || !this.weight || !this.dob) {
                return undefined;
            }
            var height = utils.parseFloat(this.height);
            var weight = utils.parseFloat(this.weight);
            var age = moment().diff(this.dob, 'years');
            if (this.gender == 'male') {
                // BMR = (10 × weight in kg) + (6.25 × height in cm) - (5 × age in years) + 5
                return (10 * weight) + (6.25 * height) - (5 * age) + 5;
            }
            else if (this.gender == 'female') {
                // BMR = (10 × weight in kg) + (6.25 × height in cm) - (5 × age in years) - 161
                return (10 * weight) + (6.25 * height) - (5 * age) - 161;
            }
            return undefined;
        },
        rmrLabel(){
            if(this.rmrSpecified){
                return this.$t('rmr') + ' (' + this.$t('estimate') + ': ' + this.rmrEstimate + ')';
            }
            else {
                return this.$t('rmr');
            }
        },
        usernameIsValid(){
            return this.username && this.username.length > 0;
        },
        newPasswordIsValid(){
            return this.newPassword && this.newPassword.length >= 6;
        },
        newPassword2IsValid(){
            return this.newPassword2 === this.newPassword;
        },
        isValid(){
            return this.usernameIsValid && this.newPasswordIsValid && this.newPassword2IsValid;
        }
    },
    watch: {
        rmrEstimate() {
            if (!this.rmrSpecified || !this.rmr ) {
                this.rmr = this.formatDecimal(this.rmrEstimate);
            }
        }
    },
    components: {},
    methods: {
        /*
        changeDoB(newVal){  
            var dob = new Date(newVal);
            console.log(newVal, dob);
            this.year = dob.getFullYear();
            this.month = dob.getMonth() + 1;
            this.day = dob.getDate();
        },
        */
        checkUsername() {
            this.usernameError = null;
            if (this.username && !this.usernameIsValid) {
                this.usernameError = this.$t('invalidUsername');
            }
        },
        checkPassword() {
            this.passwordError = null;
            this.password2Error = null;

            if (this.newPassword && this.newPassword.length < 6) {
                this.passwordError = this.$t('passwordTooShort');
            }
            if (this.newPassword && this.newPassword2 && this.newPassword !== this.newPassword2) {
                this.password2Error = this.$t('passwordsDontMatch');
            }
        },
        save() {
            //this.$ga.event('profile', 'save');
            var profile = {
                //doB: new Date(this.year, this.month.number - 1, this.day,12),
                doB: this.dob,
                gender: this.gender,
                height: this.height,
                weight: this.weight,
                rmr: this.rmr,
                pal: this.pal
            };
            this.$store.dispatch(constants.SAVE_PROFILE, {
              profile,
            }).then(_ => {
              this.notifySuccess(this.$t('saveSuccessful'));
            }).catch(_ => {
              this.notifyError(this.$t('saveFailed'));
            });
        },
        updateLogin() {
          var login = {
            username: this.username,
            oldPassword: this.oldPassword,
            newPassword: this.newPassword
          };

          this.$store.dispatch(constants.UPDATE_LOGIN, { login });
        },
        connectFacebook() {
            this.socialLogin('Facebook');
        },
        connectGoogle() {
            this.socialLogin('Google');
        },
        socialLogin(provider){
            if(this.$q.platform.is.cordova){
              if(provider == 'Google'){
                window.plugins.googleplus.login(
                  {
                    'webClientId': config.googleWebClientId
                  },
                  (obj) => {
                    if(obj.idToken){
                      api.loginWithToken('Google', obj.idToken).then(response => {
                        this.finishLogin(response.data.refreshToken, response.data.accessToken);
                      }).fail(xhr => {
                        this.changeInfo(xhr);
                      });
                    }
                  },
                  (msg) => {
                    this.changeInfo(msg);
                  }
                );
              }
              else {
                var ref = cordova.InAppBrowser.open(config.apiBaseUrl + 'users/external-login?provider='+ provider +'&client=mobile', '_blank', 'location=no');
                ref.addEventListener('loadstop', (event) => {
                  if(event.url.includes('login-success')){
                    var parts = event.url.split('/');
                    var refreshToken = parts[parts.length - 2];
                    var accessToken = parts[parts.length - 1];
      
                    ref.close();
                    if(refreshToken && accessToken){
                      this.finishLogin(refreshToken, accessToken);
                    }
                  }
                });
              }
            }
            else{
              window.location = config.apiBaseUrl + 'users/external-login?provider='+ provider +'&client=mobile&returnUrl='+ window.location;
            }
        },
        finishLogin(refreshToken, accessToken){
            this.getProfile();
        },
        changeInfo(data){
          //this.info = JSON.stringify(data);
        },
        deleteProfile() {
          this.$store.dispatch(constants.DELETE_PROFILE, { }).then(_ => {
            this.$store.dispatch(constants.LOGOUT, { }).then(_ => {
              this.$router.replace({name: 'login'});
            });
          });
        },
        getProfile(){
            this.$store.dispatch(constants.FETCH_PROFILE, { }).then(_ => {
                var profile = this.$store.state.profile.profile;
                if (profile) {
                    
                    if (profile.doB) {
                      this.dob = new Date(profile.doB);
                        /*
                        this.day = profile.doB.getDate();
                        this.month = this.months.find(m => m.number == profile.doB.getMonth() + 1);
                        this.year = profile.doB.getFullYear();
                        */
                    }
                    
                    
                    this.gender = profile.gender;
                    this.height = profile.height;
                    this.weight = profile.weight;
                    if (profile.rmr) {
                      this.rmr = profile.rmr;
                      this.rmrSpecified = true;
                    }
                    if (profile.pal) {
                      this.pal = this.pals.find(p => p.value == profile.pal);
                    }
      
                    this.hasPassword = profile.hasPassword;
                    if (profile.hasPassword) {
                        // don't show generated username
                        this.username = profile.username;
                    }
                    this.hasFacebook = profile.logins.includes('Facebook');
                    this.hasGoogle = profile.logins.includes('Google');
                }
      
                this.$store.commit(constants.LOADING_DONE);
              });
        }
    },
    created() {
      this.genders = [
        { label: '', value: '' },
        { label: this.$t('male'), value: 'male' },
        { label: this.$t('female'), value: 'female' },
      ];
      this.getProfile();
        
    }
}