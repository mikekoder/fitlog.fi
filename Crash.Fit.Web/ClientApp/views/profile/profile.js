import constants from '../../store/constants'
import utils from '../../utils'
import api from '../../api'
import formatters from '../../formatters'
import toaster from '../../toaster'
import moment from 'moment'

export default {
    data () {
        return {
            tab: 'basic',
            days: [],
            months: [],
            years: [],
            pals: [
                { value: 1.2, name: '1.2 Makaaminen s&auml;ngyssä' }
            ],

            day: undefined,
            month: undefined,
            year: undefined,
            gender: undefined,
            height: undefined,
            weight: undefined,
            rmr: undefined,
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
            password2Error: null
        }
    },
    computed: {
        dob() {
            if (this.year && this.month && this.day) {
                return new Date(this.year, this.month.number - 1, this.day)
            }
            return undefined;
        },
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
    components: {},
    methods: {
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
            self.$ga.event('profile', 'save');
            var profile = {
                doB: new Date(this.year, this.month.number - 1, this.day,12),
                gender: this.gender,
                height: this.height,
                weight: this.weight,
                rmr: this.rmr,
                pal: this.pal
            };
            self.$store.dispatch(constants.SAVE_PROFILE, {
                profile,
                success() {
                  toaster.info(self.$t('saveSuccessful'));
                },
                failure() {
                  toaster.error(self.$t('saveFailed'));
                }
            });
        },
        updateLogin() {
            var self = this;
            var login = {
                username: self.email,
                oldPassword: self.oldPassword,
                newPassword: self.newPassword
            };

            self.$store.dispatch(constants.UPDATE_LOGIN, {
                login,
                success() { },
                failure() { }
            });
        },
        connectFacebook() {
            window.location = api.baseUrl + 'users/external-login/?provider=Facebook&client=web&add=true&returnUrl=/#/profiili';
        },
        connectGoogle() {
            window.location = api.baseUrl + 'users/external-login/?provider=Google&client=web&add=true&returnUrl=/#/profiili';
        }
    },
    created() {
        var self = this;
        this.$ga.event('profile', 'open');
        this.$store.dispatch(constants.FETCH_PROFILE, {
            success() {
                var profile = self.$store.state.profile.profile;
                if (profile) {
                    if (profile.doB) {
                        self.day = profile.doB.getDate();
                        self.month = self.months.find(m => m.number == profile.doB.getMonth() + 1);
                        self.year = profile.doB.getFullYear();
                    }
                    self.gender = profile.gender;
                    self.height = profile.height;
                    self.weight = profile.weight;
                    self.rmr = profile.rmr;
                    if (profile.pal) {
                        self.pal = self.pals.find(p => p.value == profile.pal);
                    }

                    self.hasPassword = profile.hasPassword;
                    if (profile.hasPassword) {
                        // don't show generated username
                        self.email = profile.username;
                    }
                    self.hasFacebook = profile.logins.includes('Facebook');
                    self.hasGoogle = profile.logins.includes('Google');
                }

                self.$store.commit(constants.LOADING_DONE);
            }
        });
        
        moment.locale('fi');
        for (var i = 1; i <= 31; i++) {
            this.days.push(i);
        }
        this.months = moment.months().map((m, i) => { return { number: i + 1, name: m } });
        for (var i = 1900; i <= new Date().getFullYear(); i++) {
            this.years.push(i);
        }
    }
}