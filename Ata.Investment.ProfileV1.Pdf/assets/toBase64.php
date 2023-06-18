<?php

$tagBegin = '<img src="';
$tagEnd = '" class="headerImage" alt=""/>';

$directories = [
    'CapitalPreservation',
    'ExecutiveServices',
    'Safeguarding',
    'WealthAccumulation',
];

$files = [
	'page-1-cover.jpg',
	'page-2-header.jpg',
	'page-3-header.jpg',
	'page-4-header.jpg',
	'page-5-header.jpg',
	'page-6-header.jpg',
	'page-7-header.jpg',
	'page-8-header.jpg',
	'page-9-header.jpg',
	'page-10-header.jpg'
];

foreach($directories as $directory) {
    foreach($files as $file) {
        $data = file_get_contents($directory . '/' . $file);
        //$base64 = $tagBegin. 'data:image/jpg;base64,' . base64_encode($data) . $tagEnd;
        $base64 = 'data:image/jpg;base64,' . base64_encode($data);

        file_put_contents($directory . '/' . $file . ".txt", $base64);
    }
}
