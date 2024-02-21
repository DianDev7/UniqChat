// METOHD THAT SNED THE ERROR MESSAGE TO ERROR CONTROLLER
function LogError(errorMessage) {
$.ajax({
     type: 'POST',
     url: '/ErrorTextLog/LogError', 
     data: { errorMessage: errorMessage },
     success: function (response) {
     console.log('Error logged successfully:', response);
     },
     error: function (error) {
     console.error('Error logging:', error);
    }
  });
}

    

