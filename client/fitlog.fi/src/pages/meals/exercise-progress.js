import constants from '../../store/constants'
import api from '../../api'
import GraphBar from '../../components/graph-bar'
import moment from 'moment'
import graph from '../../graph'
import Help from './exercise-progress-help'
import PageMixin from '../../mixins/page'

export default {
    mixins: [PageMixin],
    components: {
        GraphBar,
        'exercise-progress-help': Help
    },
    data () {
        return {
            exercise: undefined,
            exercises: [],
            start: undefined,
            end: undefined,
            data: undefined,
            options: undefined,
            tableData: []
        }
    },
    computed:{
    },
    methods: {
        showMonths(months){
            this.end = new Date();
            this.start = moment(this.end).subtract(months, 'months').toDate();
            this.loadData();
        },
        loadData(){
            api.getExerciseHistory(this.exercise.id, this.start, this.end).then(response => {
                var data = response.data;
                if(data.length == 0){
                    this.data = undefined;
                    return;
                }
                this.tableData = data;

                var labels = data.map(d => new Date(d.time));
                var values1 = data.map(d => d.max);
                var values2 = data.map(d => d.maxBW);
                var values3 = data.map(d => d.maxInclBW);
                var values4 = data.map(d => d.totalVolume);
                var start = moment(data[0].time).startOf('day');
                var end = moment(data[data.length-1].time).endOf('day');

                var datasets = [
                    {
                        ...graph.datasets[0],
                        label: this.$t('1rm'),
                        data: values1,
                        yAxisID: '1rm',
                        type: 'line'
                    },
                    {
                        ...graph.datasets[1],
                        label: this.$t('1rmBW'),
                        data: values2,
                        yAxisID: '1rm',
                        type: 'line'
                    },
                    {
                        ...graph.datasets[2],
                        label: this.$t('1rmInclBW'),
                        data: values3,
                        yAxisID: '1rm',
                        type: 'line'
                    },
                    {
                        ...graph.datasets[3],
                        fill: true,
                        label: this.$t('volume') + '/' + this.$t('workout'),
                        data: values4,
                        yAxisID: 'volume',
                        type: 'bar'
                    }
                    ];
          
                

                this.data = {
                    labels,
                    datasets
                };

                this.options = {
                    maintainAspectRatio: false,
                    legend: {
                        display: true
                    },
                    scales: {
                        xAxes:[{
                            type: 'time',
                            time: {
                                unit: 'day',
                                displayFormats: {
                                    day: 'DD.MM.YYYY',
                                    //hour: 'HH:mm'
                                },
                                min: start,
                                max: end
                            }
                        }],
                        yAxes: [{
                            id: '1rm',
                            position: 'left',
                            scaleLabel: {
                                display: true,
                                labelString: this.$t('1rm')
                            },
                            fill: false,
                            ticks: {
                                //max: 5,
                                min: 0,
                                //max: max,
                                //stepSize: step 
                            }
                        },
                        {
                            id: 'volume',
                            //type: 'bar',
                            position: 'right',
                            scaleLabel: {
                                display: true,
                                labelString: this.$t('volume')
                            },
                            gridLines: {
                                display:false
                                //color: graph.axisColor2
                            },
                            fill: false,
                            ticks: {
                                //max: 5,
                                min: 0,
                                //max: max,
                                //stepSize: step 
                            }
                        }]
                    },
                    tooltips:{
                        callbacks:{
                            title: (tooltipItem, data) => {
                                var time = data.labels[tooltipItem[0].index];
                                return this.formatDateTime(time);
                            }
                        }
                    }
                };
                //this.data = data.map(d =>{return  {  t: new Date(d.time), y: d.value}});
            }).catch(_ => {
                this.notifyError(this.$t('fetchFailed'));
            }).finally(() => {
                this.$store.commit(constants.LOADING_DONE);
            });
        },
        showHelp(){
            this.$refs.help.open();
        }
    },
    created() {
        this.end = moment().endOf('day').toDate();
        this.start = moment(this.end).subtract(6,'month').startOf('day').toDate();
        var id = this.$route.params.exerciseId;
        this.$store.dispatch(constants.FETCH_EXERCISE, { id }).then(exercise => {
            this.exercise = exercise;
            this.loadData();
        }).catch(_ => {
            this.$store.commit(constants.LOADING_DONE);
        });
    }
}