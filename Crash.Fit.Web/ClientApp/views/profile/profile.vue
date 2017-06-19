<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t('profile') }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <!--
                    <ul class="nav nav-tabs">
                        <li v-bind:class="{ active: tab == 'basic' }"><a @click="tab='basic'">{{ $t('basic') }}</a></li>
                    </ul>-->
                </div>
            </div>
            <div class="row"></div>
            <div v-if="tab == 'basic'">
                <div class="row">
                    <div class="col-sm-12">
                        <div class="form-group dob">
                            <label>{{ $t('dob') }}</label><br />
                            <select v-model="day" class="form-control input-2">
                                <option v-for="dayOption in days" v-bind:value="dayOption">
                                    {{ dayOption }}
                                </option>
                            </select>
                            <select v-model="month" class="form-control input-10">
                                <option v-for="monthOption in months" v-bind:value="monthOption">
                                    {{ monthOption.name }}
                                </option>
                            </select>
                            <select v-model="year" class="form-control input-4">
                                <option v-for="yearOption in years" v-bind:value="yearOption">
                                    {{ yearOption }}
                                </option>
                            </select>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-group">
                            <label>{{ $t('gender') }}</label>
                            <select class="form-control input-10" v-model="gender">
                                <option v-bind:value="undefined"></option>
                                <option v-bind:value="'male'">Mies</option>
                                <option v-bind:value="'female'">Nainen</option>
                            </select>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-group">
                            <label>{{ $t('height') }}</label><br />
                            <input type="number" class="form-control input-10" v-model="height" /> cm
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-group">
                            <label>{{ $t('weight') }}</label><br />
                            <input type="number" min="1" class="form-control input-10" v-model="weight" /> kg
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-group">
                            <label>{{ $t('rmr') }}</label><br />
                            <span>{{ $t('estimate') }}: {{ rmrEstimate }}</span><br />
                            <input type="number" min="1" class="form-control input-10" v-model="rmr" /> kcal/{{ $t('day') }}
                        </div>

                    </div>
                </div>
                <!--
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-group">
                            <label>{{ $t('pal') }}</label><br />
                            <select v-model="pal" class="form-control input-25">
                                <option v-for="palOption in pals" v-bind:value="palOption">
                                    {{ palOption.name }}
                                </option>
                            </select>
                        </div>

                    </div>
                </div>-->
            </div>
            <hr />
            <div class="row">
                <div class="col-sm-6">
                    <button class="btn btn-primary" @click="save">{{ $t('save') }}</button>
                </div>
            </div>
        </section>
    </div>
</template>

<script>
    var constants = require('../../store/constants')
    var utils = require('../../utils');
    var api = require('../../api');
    var formatters = require('../../formatters')
    var toaster = require('../../toaster');
module.exports = {
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
            
        }
    },
    computed: {
        loading: function () {
            return this.$store.state.loading;
        },
        dob: function () {
            if (this.year && this.month && this.day) {
                return new Date(this.year, this.month.number - 1, this.day)
            }
            return undefined;
        },
        rmrEstimate: function () {
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
        }
    },
    components: {},
    methods: {
        
        save: function () {
            var profile = {
                doB: new Date(this.year, this.month.number - 1, this.day),
                gender: this.gender,
                rmr: this.rmr
            };
            this.$store.dispatch(constants.SAVE_PROFILE, {
                profile,
                success: function () {
                },
                failure: function () {
                }
            });
        }
    },
    created: function () {
        var self = this;
        
        this.$store.dispatch(constants.FETCH_PROFILE, {
            success: function () {
                var profile = self.$store.state.profile.profile;
                if (profile) {
                    if (profile.doB) {
                        self.day = profile.doB.getDate();
                        self.month = self.months.find(m => m.number == profile.doB.getMonth() + 1);
                        self.year = profile.doB.getFullYear();
                    }
                    self.gender = profile.gender;
                    self.rmr = profile.rmr;
                    if (profile.pal) {
                        self.pal = self.pals.find(p => p.value == profile.pal);
                    }
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
</script>

<style scoped>
    .dob .form-control {
        display:initial;
    }
    input.form-control { display:initial;}
   
    
</style>