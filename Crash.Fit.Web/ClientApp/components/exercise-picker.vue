<template>
    <input type="text" class="form-control" v-model="name" @blur="blur" />
</template>

<script>
    import api from '../api'
    require('bootstrap-3-typeahead');

export default {
    data() {
        return {
            name: null
        }
    },
    props:{
        value: {},
        exercises: null
    },
    methods: {
        blur() {
            this.$emit('nameChange', this.name);
        }
    },
    mounted() {
        if (this.value) {
            this.name = this.value.name;
            //$(this.$el).val(this.value.name);
        }
        var self = this;
        $(this.$el).typeahead({
            source(query, process) {
                var results = self.exercises.filter(e => e.name.toLowerCase().indexOf(query.toLowerCase()) >= 0);
                if (results.length == 0) {
                    results = [{ id: undefined, name: query }];
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
                self.name = exercise.name;
                self.$emit('change', exercise);
            },
            templates: {
                suggestion(data) {
                    return '<p><strong>' + data.name + '</strong> - ' + data.userId + '</p>';
                }
            }
        });
    },
    watch: {
        value(newValue) {
            if (newValue) {
                this.name = newValue.name;
                //$(this.$el).val(newValue.name);
            } else {
                this.name = '';
                //$(this.$el).val('');
            }
        }
    }
}
</script>