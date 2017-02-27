<template>
    <div></div>
</template>

<script>
var c3 = require('c3')

module.exports = {
    data: function() {
        return { }
    },
    props: {
        size: 50,
        protein: 0,
        carb: 0,
        fat: 0
    },
    mounted: function () {
        var self = this;
        var chart = c3.generate({
            bindto: self.$el,
            size: {
                height: self.size || 50,
                width: self.size || 50
            },
            padding: {
                top: 0,
                right: 0,
                bottom: 0,
                left: 0,
            },
            data: {
                columns: [
                    ['Protein', (self.protein || 0) * 4],
                    ['Carbs', (self.carb || 0) * 4],
                    ['Fat', (self.fat || 0) * 9]
                ],
                type: 'pie',
                labels: false
            },
            legend: {
                show: false
            },
            pie: {
                label: {
                    show: false
                }
            },
            color: {
                pattern: ['green', 'blue', 'red']
            }
        });
    }
}
</script>