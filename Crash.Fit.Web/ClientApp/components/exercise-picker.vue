<template>
    <input type="text" class="form-control" v-model="name" @blur="blur" />
</template>

<script>
    var api = require('../api');
    require('bootstrap-3-typeahead');

module.exports = {
    data: function() {
        return {
            name: null
        }
    },
    props:{
        value: {},
        exercises: null
    },
    methods: {
        blur: function () {
            this.$emit('nameChange', this.name);
        }
    },
    mounted: function () {
        if (this.value) {
            this.name = this.value.name;
            //$(this.$el).val(this.value.name);
        }
        var self = this;
        $(this.$el).typeahead({
            source: function (query, process) {
                var results = self.exercises.filter(e => e.name.toLowerCase().indexOf(query.toLowerCase()) >= 0);
                if (results.length == 0) {
                    results = [{ id: undefined, name: query }];
                }
                process(results);
            },
            minLength: 2,
            items: 100,
            highlighter: function (item) { return item; },
            matcher: function (item) {
                return true;
            },
            afterSelect: function (exercise) {
                self.name = exercise.name;
                self.$emit('change', exercise);
            },
            templates: {
                suggestion: function (data) {
                    return '<p><strong>' + data.name + '</strong> - ' + data.userId + '</p>';
                }
            }
        });
    },
    watch: {
        value: function (newValue) {
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