//HtmlSanitizer default doesnt inject classes so set quill to use inline style for align
const AlignStyle = Quill.import('attributors/style/align');
Quill.register(AlignStyle, true);
//adds quill to the editor div with below settings
const quill = new Quill('#editor', {
    theme: 'snow',
    modules: {
        toolbar: [
            [{ header: [1, 2, 3, 4, 5, 6, false] }],
            ['bold', 'italic', 'underline', 'strike'],
            [{ 'align': [] }],
            [{ 'list': 'ordered' }, { 'list': 'bullet' }],
            [{ 'color': [] }, { 'background': [] }],
            ['clean'],
        ]
    }
});
//the textarea note element
const noteInput = document.getElementById('Note');
//for edit dump html into quill
if (noteInput.value) {
    quill.clipboard.dangerouslyPasteHTML(noteInput.value);
}
//before submit take quill html and add it to textarea element thats bound to the model
document.getElementById('note-form').addEventListener('submit', function () {
    noteInput.value = quill.root.innerHTML;
});
//limit the quill input to 5000 and display char usage -- data model is 5000 char limit
const limit = 5000;
quill.on('text-change', function (delta, oldDelta, source) {
    if (quill.getLength() > limit) {
        quill.deleteText(limit, quill.getLength());
    }

    let remaining = limit - quill.getLength() + 1; // +1 to account for the extra newline character Quill adds
    // Update a counter element on your page
    document.querySelector('.character-count').innerText = remaining + ' Characters Remaining';
});