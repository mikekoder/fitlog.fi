<template>
    <input type="text" class="form-control" v-model="name" />
</template>

<script>
    import api from '../../api'
    require('bootstrap-3-typeahead');

    export default {
        name: 'food-picker',
        data() {
            return {
                name: null
            }
        },
        props: {
            value: {},
            disableCreation: false
        },
        methods: {
        },
        mounted() {
            if (this.value) {
                $(this.$el).val(this.value.name);
            }
            var self = this;
            $(this.$el).typeahead({
                source(query, process) {
                    api.searchFoods(query).then((results) => {

                        if (!self.disableCreation) {
                            results.push({ createFood: true, name: query, text: self.$t('createFood') });
                            results.push({ createRecipe: true, name: query, text: self.$t('createRecipe') });
                        }
                        process(results);
                    });
                },
                minLength: 2,
                items: 100,
                highlighter(item) { return item; },
                matcher(item) {
                    return true;
                },
                sorter(items) {
                    return items;
                },
                afterSelect(data) {
                    if (data.createFood) {
                        self.name = '';
                        self.$emit('createFood', data.name);
                    }
                    else if (data.createRecipe) {
                        self.name = '';
                        self.$emit('createRecipe', data.name);
                    }
                    else {
                        api.getFood(data.id).then((foodDetails) => {
                            self.$emit('change', foodDetails);
                        });
                    }
                },
                displayText(data) {
                    if (data.usageCount > 0) {
                        return '<strong>' + data.name + '</strong>';
                    }
                    if (data.createFood || data.createRecipe) {
                        return '<i class="fa fa-plus"></i> ' + data.text;
                    }
                    return data.name;
                }
            });
        },
        watch: {
            value(newValue) {
                if (newValue) {
                    this.name = newValue.name;
                } else {
                    this.name = '';
                }
            }
        }
    }
</script>