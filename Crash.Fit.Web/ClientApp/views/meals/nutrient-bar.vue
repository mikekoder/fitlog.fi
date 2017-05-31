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
           

        }
    },
    computed: {
        energy: function(){
            return 4 * this.protein + 4 * this.carb + 9 * this.fat;
        },
        proteinWidth: function () {
            return (4 * this.protein / this.energy) * 100;
        },
        carbWidth: function () {
            return (4 * this.carb / this.energy) * 100;
        },
        fatWidth: function () {
            return (9 * this.fat / this.energy) * 100;
        }
    },
    props: {
        value: undefined,
        precision: undefined
    },
    mounted: function () {
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
        background-color: #4F81BD;
    }
    .carb
    {
        background-color: #C0504D;
    }
   .fat 
   {
       background-color: #9BBB59;
   }
</style>