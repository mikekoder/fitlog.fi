<template>
    <div class="bar-container">
        <div class="bar protein" :title="$t('protein')+' ' + formatDecimal(proteinWidth, 2) +'%'" :style="'width: '+ proteinWidth +'%;'">P</div>
        <div class="bar carb" :title="$t('carbs')+' ' + formatDecimal(carbWidth, 2) +'%'" :style="'width: '+ carbWidth +'%;'">HH</div>
        <div class="bar fat" :title="$t('fat')+' ' + formatDecimal(fatWidth, 2) +'%'" :style="'width: '+ fatWidth +'%;'">R</div>
    </div>
</template>

<script>
    export default {
        name: 'energy-distribution-bar',
        data() {
            return {
            }
        },
        computed: {
            energy() {
                return 4 * this.protein + 4 * this.carb + 9 * this.fat;
            },
            proteinWidth() {
                return (4 * this.protein / this.energy) * 100;
            },
            carbWidth() {
                return (4 * this.carb / this.energy) * 100;
            },
            fatWidth() {
                return (9 * this.fat / this.energy) * 100;
            }
        },
        props: {
            protein: 0,
            carb: 0,
            fat: 0
        }
    }
</script>
<style scoped>
    .bar-container {
        width: 100%;
        height: 100%;
    }

    .bar {
        float: left;
        height: 100%;
        text-align: center;
        color: white;
    }

    .protein {
        background-color: #4F81BD;
    }

    .carb {
        background-color: #C0504D;
    }

    .fat {
        background-color: #9BBB59;
    }
</style>