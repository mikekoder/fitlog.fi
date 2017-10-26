<template>
    <div>
        <div class="bar-container" v-if="goal.min || goal.max" :title="title">
            <div class="line min" :style="minStyle" :class="minClass" v-if="goal.min"><div></div><div></div><div></div></div>
            <div class="line max" :style="maxStyle" :class="maxClass" v-if="goal.max"><div></div><div></div><div></div></div>
            <div class="bar value" :style="valueStyle"></div>
            <div class="text">{{ decimal(value, precision) }}</div>
        </div>
        <div v-else>
            {{ decimal(value, precision) }}
        </div>
    </div>
</template>

<script>
export default {
    data() {
        return {
        }
    },
    computed: {
        
        maxValue(){
            return Math.max(this.goal.min || 0, this.value || 0, this.goal.max || 0) * 1.1;
        },/*
        color() {
            
            var diff = 0;
            if (this.value || this.value == 0) {
                if (this.goal.min && this.value < this.goal.min) {
                    diff = (this.goal.min - this.value) / this.goal.min;
                }
                if (this.goal.max && this.value > this.goal.max) {
                    diff = (this.value - this.goal.max) / this.goal.max;
                }
            }
            if (diff == 0) {
                return 'green';
            }
            if (diff < 0.2) {
                return 'orange';
            }
            return '#bbbbbb';
        },
     */
        
        minLeft() {
            return (this.goal.min || 0) / this.maxValue * 100;
        },
        minStyle() {
            return 'left: ' + this.minLeft + '%;';
        },
        minClass() {
            if (this.goal.min && this.value < this.goal.min) {
                return 'bad';
            }
            if (this.goal.min && this.value > this.goal.min) {
                return 'good';
            }
            return '';
        },
        maxLeft() {
            return (this.goal.max || 0) / this.maxValue * 100;
        },
        maxStyle() {
            return 'left: ' + this.maxLeft + '%;';
        },
        maxClass() {
            if (this.goal.max && this.value > this.goal.max) {
                return 'bad';
            }
            if (this.goal.max && this.value < this.goal.max) {
                return 'good';
            }
            return '';
        },
        valueLeft() {
            return (this.value || 0) / this.maxValue * 100;
        },
        valueStyle(){
            return 'width: ' + this.valueLeft + '%; background-color: #ccc;';
        },
        title() {
            if (this.goal.min) {
                if (this.goal.max) {
                    return this.goal.min + ' - ' + this.goal.max;
                }
                else {
                    return '> ' + this.goal.min;
                }
            }
            if (this.goal.max) {
                return '< ' + this.goal.max;
            }
            return '';
        }
    },
    props: {
        goal: Object,
        value: Number,
        precision: Number
    },
    created() {

    },
    methods: {
        decimal(value, precision) {
            if (!value) {
                return value;
            }
            return value.toFixed(precision);
        }
    }
}
</script>
<style scoped>
    .bar-container {
        width: 100%;
        height: 20px;
        /*background-color: #f5f5f5;*/
        position:relative;
        cursor: default;
    }
    .bar {
        position: absolute;
        top:2px;
        height: 16px;
        z-index: 1;
    }
    .line {
        position: absolute;
        top:0;
        height: 100%;
        width: 3px;
        z-index: 2;
    }
    .line.bad > div {
        background-color: #FF0000;
    }
    .line.good > div {
        background-color: green;
    }
    .min > div, .max > div {
        position:absolute;
        background-color: #808080;
    }
    .min > div:nth-child(1){
        height: 20px;
        width: 1px;
    }
    .min > div:nth-child(2){
        left: 1px;
        height: 1px;
        width: 2px;
    }
    .min > div:nth-child(3){
        left: 1px;
        top: 19px;
        height: 1px;
        width: 2px;
    }
    .max > div:nth-child(1){
        left: 2px;
        height: 20px;
        width: 1px;
    }
    .max > div:nth-child(2){
        height: 1px;
        width: 2px;
    }
    .max > div:nth-child(3){
        top: 19px;
        height: 1px;
        width: 2px;
    }
    .text {
        position: absolute;
        top:0;
        width: 100%;
        height: 100%;
        z-index: 3;
        text-align:center;
     }
</style>