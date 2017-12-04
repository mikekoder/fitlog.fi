<template>
    <input type="text" class="form-control" v-model="name" @blur="blur" />
</template>

<script>
    import Vue from 'vue'
    import constants from '../store/constants'
    import api from '../api'
    require('bootstrap-3-typeahead');

    export default {
        data() {
            return {
                name: null
            }
        },
        props: {
            value: {},
            exercises: null
        },
        methods: {
            blur() {
                console.log('blur', this.value, this.name);
                this.$emit('nameChange', this.name);
            }
        },
        mounted() {
            var self = this;
            if (this.value) {
                $(this.$el).val(this.value.name);
            }

            $(this.$el).typeahead({
                source(query, process) {
                    var results = self.exercises.filter(e => e.name.toLowerCase().indexOf(query.toLowerCase()) >= 0);
                    if (!results.find(r => r.name.toLowerCase() == query.toLowerCase())) {
                        results.push({ id: undefined, name: query, createNew: true });
                    }
                    process(results);
                },
                minLength: 2,
                items: 100,
                highlighter(item) { return item; },
                matcher(item) {
                    return true;
                },
                afterSelect(exercise) {
                    $(self.$el).val(exercise.name);
                    if (!exercise.id) {
                        self.$store.dispatch(constants.SAVE_EXERCISE, {
                            exercise,
                            success(savedExercise) {
                                self.$emit('change', savedExercise);
                            },
                            failure() {
                            }
                        });
                    }
                    else {
                        self.$emit('change', exercise);
                    }
                },
                displayText(data) {
                    if (data.createNew) {
                        return '<i class="fa fa-plus"></i> ' + data.name;
                    }
                    return '<strong>' + data.name + '</strong>';
                }
            });
        },
        watch: {
            value(newValue) {
                console.log('watch value', newValue);
                if (newValue) {
                    this.name = newValue.name;
                } else {
                    this.name = '';
                }
            }
        }
    }
</script>