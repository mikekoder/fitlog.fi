<template>
    <input type="text" class="form-control" v-model="name" />
</template>

<script>
    var api = require('../../api');
    require('bootstrap-3-typeahead');

module.exports = {
    data: function() {
        return {
            name: null
        }
    },
    props:{
        value: {}
    },
    methods: {
    },
    mounted: function () {
        if (this.value) {
            $(this.$el).val(this.value.name);
        }
        var self = this;
        $(this.$el).typeahead({
            source: function (query, process) {
                api.searchFoods(query).then(function (results) {
                    process(results);
                });
            },
            minLength: 2,
            items: 100,
            highlighter: function (item) { return item; },
            matcher: function (item) {
                return true;
            },
            sorter: function(items){
                return items;
            },
            afterSelect: function (food) {
                api.getFood(food.id).then(function (foodDetails) {
                    self.$emit('change', foodDetails);
                });
            },
            displayText: function (data) {
                if (data.usageCount > 0) {
                    return '<strong>' + data.name + '</strong>';
                }
                return data.name;
            }
        });
    },
    watch: {
        value: function (newValue) {
            if (newValue) {
                this.name = newValue.name;
            } else {
                this.name = '';
            }
        }
    }
}
</script>