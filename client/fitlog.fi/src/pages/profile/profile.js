import constants from '../../store/constants'
import utils from '../../utils'
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

            deleteStarted: false
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
            var self = this;
            //self.$ga.event('profile', 'save');
            var profile = {
                //doB: new Date(this.year, this.month.number - 1, this.day,12),
                doB: this.dob,
                gender: this.gender,
                height: this.height,
                weight: this.weight,
                rmr: this.rmr,
                pal: this.pal
            };
            self.$store.dispatch(constants.SAVE_PROFILE, {
              profile,
            }).then(_ => {
              self.notifySuccess(self.$t('saveSuccessful'));
            }).catch(_ => {
              self.notifyError(self.$t('saveFailed'));
            });
        },
        updateLogin() {
          var self = this;
          var login = {
            username: self.username,
            oldPassword: self.oldPassword,
            newPassword: self.newPassword
          };

          self.$store.dispatch(constants.UPDATE_LOGIN, { login });
        },
        connectFacebook() {
          window.location = api.baseUrl + 'users/external-login/?provider=Facebook&client=web&add=true&returnUrl=/#/profiili';
        },
        connectGoogle() {
          window.location = api.baseUrl + 'users/external-login/?provider=Google&client=web&add=true&returnUrl=/#/profiili';
        },
        deleteProfile() {
          this.$store.dispatch(constants.DELETE_PROFILE, { }).then(_ => {
            this.$store.dispatch(constants.LOGOUT, { }).then(_ => {
              this.$router.replace({name: 'login'});
            });
          });
        }
    },
    created() {
        var self = this;
        self.genders = [
            { label: '', value: '' },
            { label: self.$t('male'), value: 'male' },
            { label: self.$t('female'), value: 'female' },
        ]
        this.$store.dispatch(constants.FETCH_PROFILE, { }).then(_ => {
            var profile = self.$store.state.profile.profile;
            if (profile) {
                
                if (profile.doB) {
                    self.dob = new Date(profile.doB);
                    /*
                    self.day = profile.doB.getDate();
                    self.month = self.months.find(m => m.number == profile.doB.getMonth() + 1);
                    self.year = profile.doB.getFullYear();
                    */
                }
                
                
                self.gender = profile.gender;
                self.height = profile.height;
                self.weight = profile.weight;
                if (profile.rmr) {
                    self.rmr = profile.rmr;
                    self.rmrSpecified = true;
                }
                if (profile.pal) {
                    self.pal = self.pals.find(p => p.value == profile.pal);
                }

                self.hasPassword = profile.hasPassword;
                if (profile.hasPassword) {
                    // don't show generated username
                    self.username = profile.username;
                }
                self.hasFacebook = profile.logins.includes('Facebook');
                self.hasGoogle = profile.logins.includes('Google');
            }

            self.$store.commit(constants.LOADING_DONE);
        });
    }
}