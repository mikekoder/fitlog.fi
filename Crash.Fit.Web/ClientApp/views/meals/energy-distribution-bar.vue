<template>
    <div class="bar-container">
        <div class="bar protein" :title="'Proteiini ' + decimal(proteinWidth, 2) +'%'" :style="'width: '+ proteinWidth +'%;'">P</div>
        <div class="bar carb" :title="'Hiilihydraatti ' + decimal(carbWidth, 2) +'%'" :style="'width: '+ carbWidth +'%;'">HH</div>
        <div class="bar fat" :title="'Rasva ' + decimal(fatWidth, 2) +'%'" :style="'width: '+ fatWidth +'%;'">R</div>
    </div>
</template>

<script>
module.exports = {
    data: function() {
        return {
            proteinWidth: null,
            carbWidth: null,
            fatWidth: null
        }
    },
    props: {
        size: 50,
        protein: 0,
        carb: 0,
        fat: 0
    },
    mounted: function () {
        var self = this;
        var energy = 4 * this.protein + 4 * this.carb + 9 * this.fat;
        this.proteinWidth = (4 * this.protein / energy) * 100;
        this.carbWidth = (4 * this.carb / energy) * 100;
        this.fatWidth = (9 * this.fat / energy) * 100;
    },
    methods: {
        decimal: function (value, precision) {
            if (!value) {
                return value;
            }
            return value.toFixed(precision);
        }
    }
}
</script>
<style scoped>
    .bar-container
    {
        width: 100%;
        height: 100%;
    }
    .bar
    {
        float:left;
        height: 100%;
        text-align: center;
        color:white;
    }
    .protein 
    {
        background-color: red;
    }
    .carb
    {
        background-color: green;
    }
   .fat 
   {
       background-color: blue;
   }
</style>