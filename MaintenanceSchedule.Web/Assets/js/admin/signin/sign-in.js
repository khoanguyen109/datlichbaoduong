
jQuery(document).ready(function() {
	
    /*
        Fullscreen background
    */
    $.backstretch("/assets/images/login-bg.jpg");
    
    /*
        Form validation
    */
    $('.login-form input[type="text"], .login-form input[type="password"]').on('focus', function() {
    	$(this).removeClass('input-error');
    });
    
    $('.login-form').on('submit', function(e) {
    	$(this).find('input[type="text"], input[type="password"]').each(function() {
    		if( $(this).val() == "" ) {
    			e.preventDefault();
    			$(this).addClass('input-error');
    		}
    		else {
    			$(this).removeClass('input-error');
    		}
    	});
    });
});
