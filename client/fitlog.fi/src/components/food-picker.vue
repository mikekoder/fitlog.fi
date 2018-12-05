<template>
    <input type="text" class="form-control" v-model="name" />
</template>

<script>
    import $ from 'jquery'
    import api from '../api'
    require('bootstrap-3-typeahead');

export default {
    data() {
        return {
            name: null
        }
    },
    props:{
        value: {}
    },
    methods: {
    },
    mounted () {
        if (this.value) {
            $(this.$el).val(this.value.name);
        }
        var self = this;
        $(this.$el).typeahead({
            source (query, process) {
                api.searchFoods(query).then(response => {
                    process(response.data);
                });
            },
            minLength: 2,
            items: 100,
            highlighter (item) { return item; },
            matcher (item) {
                return true;
            },
            sorter(items){
                return items;
            },
            afterSelect (food) {
                api.getFood(food.id).then(response => {
                    self.$emit('change', response.data);
                });
            },
            displayText (data) {
                if (data.usageCount > 0) {
                    return '<strong>' + data.name + '</strong>';
                }
                return data.name;
            }
        });
    },
    watch: {
        value (newValue) {
            if (newValue) {
                this.name = newValue.name;
            } else {
                this.name = '';
            }
        }
    }
}
</script>