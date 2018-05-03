import constants from '../../store/constants'
import api from '../../api'
import GraphBar from '../../components/graph-bar'
import moment from 'moment'
import graph from '../../graph'
import Help from './exercise-progress-help'

export default {
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
            var self = this;
            self.end = new Date();
            self.start = moment(self.end).subtract(months, 'months').toDate();
            self.loadData();
        },
        loadData(){
            var self = this;
            api.getExerciseHistory(self.exercise.id, self.start, self.end).done(data => {
                if(data.length == 0){
                    self.data = undefined;
                    return;
                }
                self.tableData = data;

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
                        label: self.$t('1rm'),
                        data: values1,
                        yAxisID: '1rm',
                        type: 'line'
                    },
                    {
                        ...graph.datasets[1],
                        label: self.$t('1rmBW'),
                        data: values2,
                        yAxisID: '1rm',
                        type: 'line'
                    },
                    {
                        ...graph.datasets[2],
                        label: self.$t('1rmInclBW'),
                        data: values3,
                        yAxisID: '1rm',
                        type: 'line'
                    },
                    {
                        ...graph.datasets[3],
                        fill: true,
                        label: self.$t('volume') + '/' + self.$t('workout'),
                        data: values4,
                        yAxisID: 'volume',
                        type: 'bar'
                    }
                    ];
          
                

                self.data = {
                    labels,
                    datasets
                };

                self.options = {
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
                                labelString: self.$t('1rm')
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
                                labelString: self.$t('volume')
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
                            title: function(tooltipItem, data) {
                                var time = data.labels[tooltipItem[0].index];
                                return self.formatDateTime(time);
                            }
                        }
                    }
                };
                //self.data = data.map(d =>{return  {  t: new Date(d.time), y: d.value}});
            }).fail(() => {
                self.notifyError(self.$t('fetchFailed'));
            }).always(() => {
                self.$store.commit(constants.LOADING_DONE);
            });
        },
        showHelp(){
            this.$refs.help.open();
        }
    },
    created() {
        var self = this;
        self.end = new Date();
        self.start = moment(self.end).subtract(6,'month').toDate();
        self.$store.dispatch(constants.FETCH_EXERCISES, {
            success(exercises){
                self.exercises = exercises.map(e => {return {...e, label: e.name, value: e }});
                var exerciseId = self.$route.params.exerciseId;
                self.exercise = self.exercises.find(e => e.id == exerciseId);
            
                self.loadData();
            },
            failure(){
              self.$store.commit(constants.LOADING_DONE);
            }
        });
    }
}