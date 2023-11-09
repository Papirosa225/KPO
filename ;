#include <QApplication> // Включаем необходимые библиотеки

#include <QWidget> // Инициализируем виджет

#include <QPushButton> // Добавляем кнопку

#include <QLabel> // Используем метку для текста

#include <QSpinBox> // Для ввода числовых значений

#include <QSlider> // Добавляем слайдер

#include <QTextEdit> // Для отображения и редактирования текста

#include <QGridLayout> // Используем сетку для удобного расположения элементов интерфейса

#include <QFileDialog> // Открываем диалоговое окно для выбора файла

#include <QFont> // Манипулируем шрифтом

int main(int argc, char **argv) { // Начало основной функции

    QApplication app(argc, argv); // Создаем объект приложения

    //1 Widget
    QWidget widget(0); // Инициализируем виджет

    QVBoxLayout mainLayout; // Организуем вертикальное расположение элементов
    widget.setLayout(&mainLayout); // Применяем компоновку к виджету

    //1.1 Color Mixer Section
    QHBoxLayout colorLayout; // Располагаем элементы в строку

    QFrame colorFrame; // Создаем рамку

    colorFrame.setFrameShape(QFrame::StyledPanel); // Задаем стиль для рамки
    colorFrame.setFixedSize(200, 200); // Устанавливаем размеры рамки

    QGridLayout controlLayout; // Организуем сетку для элементов управления

    QLabel titleLabel("<h2>Color Mixer</h2>"); // Заголовок для нашего интерфейса
    titleLabel.setAlignment(Qt::AlignCenter); // Выравниваем по центру

    QLabel sb_label1("<b><font color='red'>Red:</font></b>"); // Метка для значения Red
    QLabel sb_label2("<b><font color='green'>Green:</font></b>"); // Метка для значения Green
    QLabel sb_label3("<b><font color='blue'>Blue:</font></b>"); // Метка для значения Blue

    QSpinBox sbRed, sbGreen, sbBlue; // Ползунки для красного, зеленого и синего цветов
    QSlider slRed(Qt::Horizontal), slGreen(Qt::Horizontal), slBlue(Qt::Horizontal); // Ползунки для управления цветами

    colorLayout.addWidget(&colorFrame); // Добавляем рамку в расположение цвета
    controlLayout.addWidget(&titleLabel, 0, 0, 1, 3); // Добавляем заголовок в сетку

    controlLayout.addWidget(&sb_label1, 1, 0); // Добавляем метку для Red
    controlLayout.addWidget(&sbRed, 1, 1); // Добавляем ползунок для Red
    controlLayout.addWidget(&slRed, 1, 2); // Добавляем слайдер для Red

    controlLayout.addWidget(&sb_label2, 2, 0); // Добавляем метку для Green
    controlLayout.addWidget(&sbGreen, 2, 1); // Добавляем ползунок для Green
    controlLayout.addWidget(&slGreen, 2, 2); // Добавляем слайдер для Green

    controlLayout.addWidget(&sb_label3, 3, 0); // Добавляем метку для Blue
    controlLayout.addWidget(&sbBlue, 3, 1); // Добавляем ползунок для Blue
    controlLayout.addWidget(&slBlue, 3, 2); // Добавляем слайдер для Blue

    mainLayout.addLayout(&colorLayout); // Добавляем расположение цвета в основное расположение
    mainLayout.addLayout(&controlLayout); // Добавляем сетку с элементами управления в основное расположение

    // Устанавливаем диапазоны значений
    sbRed.setMinimum(0);
    sbRed.setMaximum(255);
    slRed.setMinimum(0);
    slRed.setMaximum(255);

    sbGreen.setMinimum(0);
    sbGreen.setMaximum(255);
    slGreen.setMinimum(0);
    slGreen.setMaximum(255);

    sbBlue.setMinimum(0);
    sbBlue.setMaximum(255);
    slBlue.setMinimum(0);
    slBlue.setMaximum(255);

    // Лямбда-функция для установки цвета фона
    auto setColor = [&]() {
        int red = sbRed.value();
        int green = sbGreen.value();
        int blue = sbBlue.value();
        QString styleSheet = QString("background-color: rgb(%1, %2, %3);")
                                 .arg(red)
                                 .arg(green)
                                 .arg(blue);
        colorFrame.setStyleSheet(styleSheet);
    };

    // Соединяем сигналы и слоты
    QObject::connect(&slRed, &QSlider::valueChanged, &sbRed, &QSpinBox::setValue);
    QObject::connect(&sbRed, QOverload<int>::of(&QSpinBox::valueChanged), &slRed, &QSlider::setValue);
    QObject::connect(&slRed, &QSlider::valueChanged, setColor);

    QObject::connect(&slGreen, &QSlider::valueChanged, &sbGreen, &QSpinBox::setValue);
    QObject::connect(&sbGreen, QOverload<int>::of(&QSpinBox::valueChanged), &slGreen, &QSlider::setValue);
    QObject::connect(&slGreen, &QSlider::valueChanged, setColor);

    QObject::connect(&slBlue, &QSlider::valueChanged, &sbBlue, &QSpinBox::setValue);
    QObject::connect(&sbBlue, QOverload<int>::of(&QSpinBox::valueChanged), &slBlue, &QSlider::setValue);
    QObject::connect(&slBlue, &QSlider::valueChanged, setColor);

    widget.setWindowTitle("Color Mixer"); // Задаем заголовок для окна
    widget.setStyleSheet("background-color: lightgray;"); // Задаем стиль для фона виджета
    widget.show(); // Отображаем виджет на экране

    //2 Widget
    QWidget widgetFileViewer(0); // Инициализируем второй виджет
    QVBoxLayout mainLayoutFileViewer; // Располагаем элементы в вертикальном положении
    widgetFileViewer.setLayout(&mainLayoutFileViewer); // Применяем компоновку к виджету

    QTextEdit text_edit_FileViewer; // Инициализируем поле для отображения текста
    text_edit_FileViewer.setStyleSheet("background-color: white; color: black; border: 2px solid black;"); // Устанавливаем стиль для поля

    QPushButton openButton("Choose File"); // Создаем кнопку
    openButton.setStyleSheet("background-color: lightblue; font-weight: bold;"); // Устанавливаем стиль для кнопки

    mainLayoutFileViewer.addWidget(&text_edit_FileViewer); // Добавляем поле для отображения текста в расположение
    mainLayoutFileViewer.addWidget(&openButton); // Добавляем кнопку в расположение

    auto openFile = [&](){ // Лямбда-функция для открытия файла
        QString filePath = QFileDialog::getOpenFileName(&widgetFileViewer, "Choose File", "", "Text Files (*.txt)"); // Открываем диалоговое окно для выбора файла
        if (!filePath.isEmpty()) // Если путь к файлу не пустой
        {
            QFile file(filePath); // Открываем файл по указанному пути
            if (file.open(QIODevice::ReadOnly | QIODevice::Text)) // Если файл открыт для чтения
            {
                QTextStream in(&file); // Инициализируем поток для чтения из файла
                QString fileContents = in.readAll(); // Считываем содержимое файла
                file.close(); // Закрываем файл
                text_edit_FileViewer.setPlainText(fileContents); // Отображаем содержимое файла
            }
            else // Если файл не удалось открыть
            {
                text_edit_FileViewer.setPlainText("Error opening file: " + file.errorString()); // Выводим сообщение об ошибке
            }
        }

    };

    QObject::connect(&openButton, &QPushButton::clicked, openFile); // Соединяем сигнал нажатия на кнопку с открытием файла

    widgetFileViewer.setWindowTitle("File Viewer"); // Задаем заголовок для второго окна
    widgetFileViewer.setStyleSheet("background-color: lightgray;"); // Задаем стиль для фона второго виджета
    widgetFileViewer.show(); // Отображаем второй виджет

    return app.exec(); // Запускаем главный цикл приложения
}
