<template>
    <input type="text" class="form-control" v-model="text" />
</template>

<script>
    import api from '../../api'
    require('bootstrap-3-typeahead');

    export default {
        name: 'food-picker',
        data() {
            return {
                name: null,
                manufacturer: null
            }
        },
        computed: {
            text: {
                get() {
                    return this.getDisplayText(this.name, this.manufacturer);
                },
                set(val){}
            }
        },
        props: {
            value: {},
            disableCreation: false
        },
        methods: {
            getDisplayText(name, manufacturer) {
                if (manufacturer) {
                    return name + ' (' + manufacturer + ')';
                }
                return name;
            }
        },
        mounted() {
            if (this.value) {
                this.name = this.value.name;
                this.manufacturer = this.value.manufacturer;

                //$(this.$el).val(this.text);
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
                        $(self.$el).val(self.getDisplayText(data.name, data.manufacturer));
                        api.getFood(data.id).then((foodDetails) => {
                            self.$emit('change', foodDetails);
                        });
                    }
                },
                displayText(data) {
                    if (data.createFood || data.createRecipe) {
                        return '<i class="fa fa-plus"></i> ' + data.text;
                    }
                    var text = data.name;
                    if (data.manufacturer) {
                        text += ' (' + data.manufacturer +')'
                    }
                    if (data.usageCount > 0) {
                        return '<strong>' + text + '</strong>';
                    }
                    
                    return text;
                }
            });
        },
        watch: {
            value(newValue) {
                if (newValue) {
                    this.name = newValue.name;
                    this.manufacturer = newValue.manufacturer;
                } else {
                    this.name = '';
                    this.manufacturer = '';
                }
            }
        }
    }
</script>