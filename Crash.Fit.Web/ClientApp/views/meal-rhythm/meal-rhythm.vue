<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("mealRhythm") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <div class="row hidden-xs">
                        <div class="col-sm-6 col-text-30">{{ $t('mealName') }}</div>
                        <div class="col-sm-2 col-time">{{ $t('starts') }}</div>
                        <div class="col-sm-2 col-time">{{ $t('ends') }}</div>
                        <div class="col-sm-2 col-actions-3"></div>
                    </div>
                    <template v-for="(def, index) in definitions">
                        <div class="row">
                            <div class="col-sm-6 col-text-30">
                                <label class="hidden-sm hidden-md hidden-lg">{{ $t('mealName') }}</label>
                                <input class="form-control" v-model="def.name" />
                            </div>
                            <div class="col-xs-6 col-time">
                                <label class="hidden-sm hidden-md hidden-lg">{{ $t('starts') }}</label>
                                <datetime-picker v-bind:value="def.start" v-on:change="def.start=arguments[0]" v-bind:format="'HH:mm'"></datetime-picker>
                            </div>
                            <div class="col-xs-6 col-time">
                                <label class="hidden-sm hidden-md hidden-lg">{{ $t('ends') }}</label>
                                <datetime-picker v-bind:value="def.end" v-on:change="def.end=arguments[0]" v-bind:format="'HH:mm'"></datetime-picker>
                            </div>
                            <div class="col-xs-12 col-actions-1">
                                <label class="hidden-sm hidden-md hidden-lg">&nbsp;</label>
                                <button class="btn btn-sm btn-danger" @click="deleteMeal(index)">{{ $t('delete') }}</button>
                            </div>
                        </div>
                        <div class="row meal-row-separator hidden-sm hidden-md hidden-lg">
                                <div class="col-sm-12"><hr /></div>
                        </div>
                    </template>
                    
                    <div class="row table-actions">
                        <div class="col-sm-6"><button class="btn" @click="addMeal">{{ $t('add') }}</button></div>
                        <div class="col-sm-6"></div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-6 col-text-30">
                        <label class="hidden-sm hidden-md hidden-lg">{{ $t('otherTimes') }}</label>
                        <input class="form-control" v-model="othersName" /></div>
                        <div class="col-sm-4 hidden-xs">{{ $t('otherTimes') }}</div>
                    </div>
                    
                    
                </div>
            </div>
            <hr />
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save">{{ $t('save')}}</button>
                </div>
            </div>
        </section>
    </div>
</template>

<script>
    import constants from '../../store/constants'
    import api from '../../api'
    import formatters from '../../formatters'
    import toaster from '../../toaster'

export default {
    data () {
        return {
            othersId: '',
            othersName: '',
            definitions: []
        }
    },
    computed: {
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker')
    },
    methods: {
        addMeal() {
            this.definitions.push({start: '01.01.2000 10:00', end: '01.01.2000 13:00'});
        },
        deleteMeal(index) {
            this.definitions.splice(index, 1);
        },
        save() {
            var self = this;
            var defs = self.definitions.filter(d => d.name && d.start && d.end).map(d => { return { id: d.id, name: d.name, start: self.formatTime(d.start), end: self.formatTime(d.end) } });
            defs.push({ id: self.othersId, name: self.othersName });
            self.$store.dispatch(constants.SAVE_MEAL_DEFINITIONS, {
                definitions: defs,
                success() {
                    toaster.info(self.$t('saved'));
                },
                failure() {
                    toaster.error(self.$t('saveFailed'));
                }
            });
        },
        formatTime: formatters.formatTime
    },
    created() {
        var self = this;
        self.$store.dispatch(constants.FETCH_MEAL_DEFINITIONS, {
            success(definitions) {
                self.definitions = definitions.filter(d => d.startHour).map(d => { return { id: d.id, name: d.name, start: '01.01.2000 ' + d.startHour + ':' + d.startMinute, end: '01.01.2000 ' + d.endHour + ':' + d.endMinute } });
                var other = definitions.find(d => !d.startHour);
                if (other) {
                    self.othersId = other.id;
                    self.othersName = other.name;
                }
                self.$store.commit(constants.LOADING_DONE);
            },
            failure() {
                toaster.error(self.$t('fetchFailed'));
            }
        });
        
    }
}
</script>

<style scoped>
    div.meal-row {
        margin-bottom: 5px;
    }
    div.meal-row-separator hr {
        border: 1px solid #00c0ef;
    }
</style>