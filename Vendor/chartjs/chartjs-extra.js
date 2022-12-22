//--------- CHART JS --------- //

Chart.defaults.global.legend.labels.usePointStyle = true;
var ctx = $("#static-data-graph");
var completedColor = '#448AFF';
var remainColor = '#FF6A7C';
var chart = new Chart(ctx, {
  // The type of chart we want to create
  type: 'horizontalBar',

  // The data for our dataset
  data: {
    labels: ['FO Approve Transaction','BO Allocate Fund','BO Allocate Fund 2','FO Approve Transaction','BO Allocate Fund','BO Allocate Fund 2'],
    datasets: [
      {
      label: "Remaining",
      backgroundColor: remainColor,
      borderColor: remainColor,
      data: [6, 145, 0,6, 145, 0],
    },
    {
      label: "Completed",
      backgroundColor: completedColor,
      borderColor: completedColor,
      data: [2,100,0,2,100],
    }
  ]
  },

  // Configuration options go here
  options: {
    responsive: true,
    //maintainAspectRatio: false,
      legend: {
        display: true,
        //position: 'right',
    },
    scales: {
      yAxes: [{
          barPercentage: 0.6,
          categoryPercentage: 0.7,
          //maxBarThickness: 10,
          gridLines: {
            display: false
          },
          ticks: {
            fontColor: "#446574", // this here
          },
      }],
      xAxes: [{
        gridLines: {
          color: '#F0F3F7',
        },
        ticks: {
          fontColor: "#7DA1B1", // this here
        },
    }],

  },
  }
});

//$(ctx).attr("height", dataSet.length * 20);